using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using VisionClient.Core.Enums;
using VisionClient.Core.Helpers;

namespace VisionClient.Core.Services
{
    public abstract class HttpService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IStaticData staticData;

        protected HttpService(IHttpClientFactory httpClientFactory, IStaticData staticData)
        {
            this.httpClientFactory = httpClientFactory;
            this.staticData = staticData;
        }

        public async Task<T?> SendAsync<T>(ApiRequest apiRequest)
        {
            apiRequest.Access_Token = staticData.UserData.Access_Token;
            var client = httpClientFactory.CreateClient();
            HttpRequestMessage request = new();
            request.Headers.Add("Accept", "application/json");

            request.RequestUri = new Uri(apiRequest.ApiUrl);
            client.DefaultRequestHeaders.Clear();

            if (apiRequest.Data is not null) request.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(apiRequest.Access_Token)) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Access_Token);

            HttpResponseMessage apiResponse;

            request.Method = apiRequest.ApiType switch
            {
                APIType.GET => HttpMethod.Get,
                APIType.POST => HttpMethod.Post,
                APIType.DELETE => HttpMethod.Delete,
                APIType.PUT => HttpMethod.Put,
                _ => HttpMethod.Get,
            };

            apiResponse = await client.SendAsync(request);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            if (apiContent is not null)
            {
                var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);
                if (apiResponseDto is not null) return apiResponseDto;
            }

            return default;
        }
    }
}
