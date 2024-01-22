//namespace AssignmentAPI.Service
//{
//    public class NewsService
//    {
//    }
//}

using System.Net.Http;
using System.Threading.Tasks;
using AssignmentAPI.Model;
using Newtonsoft.Json;
using System.Net.Http;

namespace AssignmentAPI.Service
{
    public class NewsService
    {
        private readonly HttpClient _httpClient;

        public NewsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<NewsItem> GetNewsItemAsync(string storyId)
        {
            var response = await _httpClient.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json?print=pretty");
            return JsonConvert.DeserializeObject<NewsItem>(response);
        }
        public async Task<string> GetTopStoriesAsync()
        {
            var topStoriesUrl = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";
            return await _httpClient.GetStringAsync(topStoriesUrl);
        }
    }
}

