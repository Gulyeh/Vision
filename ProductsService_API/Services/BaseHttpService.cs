using Newtonsoft.Json;
using ProdcutsService_API.Helpers;
using ProductsService_API.Helpers;
using ProductsService_API.Services.IServices;
using System.Net.Http.Headers;
using System.Text;

namespace ProductsService_API.Services
{
    public class BaseHttpService : IBaseHttpService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public BaseHttpService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(true);
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
            switch (apiRequest.apiType)
            {
                case APIType.GET:
                    request.Method = HttpMethod.Get;
                    break;
                case APIType.POST:
                    request.Method = HttpMethod.Post;
                    break;
                case APIType.DELETE:
                    request.Method = HttpMethod.Delete;
                    break;
                default:
                    request.Method = HttpMethod.Get;
                    break;
            }

            apiResponse = await client.SendAsync(request);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            if (apiContent is not null)
            {
                var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);
                if (apiResponseDto is not null) return apiResponseDto;
            }
            return default(T);
        }
    }
}