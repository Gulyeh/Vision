using Newtonsoft.Json;
using ProdcutsService_API.Helpers;
using ProductsService_API.Helpers;
using ProductsService_API.Services.IServices;
using System.Net.Http.Headers;
using System.Text;

namespace ProductsService_API.Services
{
    public abstract class BaseHttpService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<BaseHttpService> logger;

        public BaseHttpService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }
        
        public async Task<T?> SendAsync<T>(ApiRequest apiRequest)
        {
            var client = httpClientFactory.CreateClient("MessageService_API");
            HttpRequestMessage request = new HttpRequestMessage();
            request.Headers.Add("Accept", "application/json");
            request.RequestUri = new Uri(apiRequest.ApiUrl);
            client.DefaultRequestHeaders.Clear();

            if (apiRequest.Data is not null) request.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(apiRequest.Access_Token)) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Access_Token);

            HttpResponseMessage apiResponse;
            request.Method = apiRequest.apiType switch
            {
                APIType.GET => HttpMethod.Get,
                APIType.POST => HttpMethod.Post,
                APIType.DELETE => HttpMethod.Delete,
                _ => HttpMethod.Get,
            };

            apiResponse = await client.SendAsync(request);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            if (apiContent is not null)
            {
                logger.LogInformation("Received data from '{url}'", apiRequest.ApiUrl);
                var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);
                if (apiResponseDto is not null) return apiResponseDto;
                logger.LogInformation("Could not parse data from '{url}'", apiRequest.ApiUrl);
            }

            logger.LogInformation("Received data from '{url}' was empty", apiRequest.ApiUrl);
            return default(T);
        }
    }
}