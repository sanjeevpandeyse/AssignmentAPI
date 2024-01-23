using AssignmentAPI.Model;
using AssignmentAPI.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http;


namespace AssignmentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchStoryController : ControllerBase
    {
        private readonly NewsService _newsService;
        private readonly IMemoryCache _cache;

        public SearchStoryController(NewsService newsService, IMemoryCache cache)
        {
            _newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        /// Gets a list of news items.
        /// </summary>
        /// <returns>An action result containing a list of news items.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsItem>>> GetNewsItems()
        {
            try
            {
                if (!_cache.TryGetValue("TopStories", out List<string> topStories))
                {
                    var topStoriesResponse = await _newsService.GetTopStoriesAsync();
                    if (topStoriesResponse == null)
                    {
                        return StatusCode(500, "Unable to fetch top stories from the API.");
                    }

                    topStories = JsonConvert.DeserializeObject<List<string>>(topStoriesResponse) ?? new List<string>();

                    if (topStories == null)
                    {
                        return StatusCode(500, "Top stories list is null.");
                    }

                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    };

                    _cache.Set("TopStories", topStories, cacheEntryOptions);
                }

                var storyIds = topStories.Take(200).ToList();

                var newsItemTasks = storyIds.Select(storyId => _newsService.GetNewsItemAsync(storyId)).ToList();
                await Task.WhenAll(newsItemTasks);

                var newsItems = newsItemTasks.Select(task => task.Result).ToList();

                return newsItems;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}
