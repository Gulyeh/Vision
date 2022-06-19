using Newtonsoft.Json;
using VisionClient.Core.Dtos;

namespace VisionClient.Core.Helpers
{
    public static class ResponseToJsonHelper
    {
        public static T GetJson<T>(ResponseDto data) where T : new()
        {
            if (!data.isSuccess) return new T();

            var list = data.Response?.ToString();
            if (string.IsNullOrEmpty(list)) return new T();

            var json = JsonConvert.DeserializeObject<T>(list);

            if (json is null) return new T();
            return json;
        }

        public static string GetJson(ResponseDto data)
        {
            var responseString = data.Response.ToString()?.Replace("[", "").Replace("]", "");
            if (!string.IsNullOrWhiteSpace(responseString))
            {
                var responseDeserialized = JsonConvert.DeserializeObject<string>(responseString);
                if (string.IsNullOrWhiteSpace(responseDeserialized)) return string.Empty;
                return responseDeserialized;
            }

            return string.Empty;
        }
    }
}
