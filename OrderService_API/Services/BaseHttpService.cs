using Newtonsoft.Json;
using OrderService_API.Dtos;
using OrderService_API.Helpers;
using System.Net.Http.Headers;
using System.Text;

namespace OrderService_API.Services
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
            try
            {
                var client = httpClientFactory.CreateClient("OrderService_API");
                HttpRequestMessage request = new HttpRequestMessage();
                request.Headers.Add("Accept", "application/json");
                request.RequestUri = new Uri(apiRequest.ApiUrl);
                client.DefaultRequestHeaders.Clear();

                if (apiRequest.Data is not null) request.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
                if (!string.IsNullOrEmpty(apiRequest.Access_Token)) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Access_Token.Replace("Bearer ", ""));

                HttpResponseMessage apiResponse;
                request.Method = apiRequest.apiType switch
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
                    logger.LogInformation("Received data from '{url}'", apiRequest.ApiUrl);
                    var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                    if (apiResponseDto is not null && apiResponseDto.isSuccess)
                    {
                        var response = apiResponseDto.Response.ToString();
                        if (!string.IsNullOrWhiteSpace(response))
                        {
                            var parsedResponse = JsonConvert.DeserializeObject<T>(response);
                            if (parsedResponse is not null) return parsedResponse;
                        }
                    }
                    logger.LogInformation("Could not parse data from '{url}'", apiRequest.ApiUrl);
                }

                logger.LogInformation("Received data from '{url}' was empty", apiRequest.ApiUrl);
                return default(T);
            }
            catch (Exception)
            {
                logger.LogInformation("Could not parse data from '{url}'", apiRequest.ApiUrl);
                return default(T);
            }
        }
    }
}