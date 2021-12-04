using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using FluentAssertions;
using OneOf;

namespace Test.IntegrationTests.Extensions
{
    public static class IntegrationTestClientExtensions
    {
        public static async Task<OneOf<T, HttpResponseMessage>> GetAsyncOneOf<T>(this HttpClient client, string route)
        {
            var response = await client.GetAsync(route);
            if (!response.IsSuccessStatusCode)
                return new HttpResponseMessage(response.StatusCode);

            var stringResult = await response.Content.ReadAsStringAsync();
            return stringResult.ToDeserialize<T>();
        }

        public static async Task<T> GetAsync<T>(this HttpClient client, string route)
        {
            var response = await client.GetAsync(route);
            var stringResult = await CheckAndReturnResponseContent(response);
            return stringResult.ToDeserialize<T>();
        }

        public static async Task<TResult> PostAsync<TPost, TResult>(this HttpClient client, string route, TPost data)
        {
            var stringContent = new StringContent(data.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(route, stringContent);
            var stringResult = await CheckAndReturnResponseContent(response);
            return stringResult.ToDeserialize<TResult>();
        }

        public static async Task PostAsync<TPost>(HttpClient client, string route, TPost data)
        {
            var stringContent = new StringContent(data.ToJson(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(route, stringContent);
            await CheckAndReturnResponseContent(response);
        }

        private static async Task<string> CheckAndReturnResponseContent(HttpResponseMessage response)
        {
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return result;

            response.IsSuccessStatusCode.Should().BeTrue(result);
            return result;
        }
    }

    //public struct Response
    //{
    //    public Response(HttpResponseMessage response)
    //    {
    //        Response = response;
    //    }

    //    public HttpResponseMessage Response { get; }
    //}
}
