using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace Extensions
{
    public static class HttpClientExtension
    {
        private static readonly ILogger Logger = Log.ForContext(typeof(HttpClientExtension));


        public static async Task<string> HttpGetAsync(this HttpClient client, string route)
        {
            var url = $"{client.BaseAddress}{route} ";
            Logger.Information("End Point URL: {0}", url);

            var response = await client.GetAsync(route);
            await response.EnsureSuccessStatusCodeAsync();
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<T> HttpGetAsync<T>(this HttpClient client, string route)
        {
            var url = $"{client.BaseAddress}{route} ";
            Logger.Information("End Point URL: {0}", url);

            var response = await client.GetAsync(route);
            await response.EnsureSuccessStatusCodeAsync();
            var stringResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResult);
        }

        public static async Task<T> HttpGetAsyncForActionResult<T>(this HttpClient client, string route)
        {
            var url = $"{client.BaseAddress}{route} ";
            Logger.Information("End Point URL: {0}", url);

            var response = await client.GetAsync(route);
            var stringResult = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResult);
        }

        public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized))
                    throw new Exception(System.Net.HttpStatusCode.Unauthorized.ToString());
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

        public static void ConfigureWithAuthToken(this HttpClient client, string baseUrl, string authToken)
        {
            client.ConfigureAddressAndCommonHeaders(baseUrl);

            //load auth token in header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }

       
        public static void ConfigureAddressAndCommonHeaders(this HttpClient client, string baseUrl)
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
