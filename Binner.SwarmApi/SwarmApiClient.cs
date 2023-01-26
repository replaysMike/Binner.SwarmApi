using Binner.SwarmApi.Extensions;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Text;

namespace Binner.SwarmApi
{
    /// <summary>
    /// Swarm api client
    /// <see cref="https://swarm.binner.io"/>
    /// </summary>
    public class SwarmApiClient : ISwarmApiClient
    {
        private readonly SwarmApiConfiguration _configuration = new SwarmApiConfiguration();
        private static readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() => CreateHttpClient());
        private enum Endpoints
        {
            [Description("part/search")]
            Search,
            [Description("part/info")]
            Info
        };

        /// <summary>
        /// Create a Swarm api client
        /// <see cref="https://swarm.binner.io"/>
        /// </summary>
        public SwarmApiClient()
        {
        }

        /// <summary>
        /// Create a Swarm api client
        /// <see cref="https://swarm.binner.io"/>
        /// </summary>
        /// <param name="configuration">Configuration to apply</param>
        public SwarmApiClient(SwarmApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Search parts
        /// </summary>
        /// <param name="partNumber">The part number to search</param>
        public Task<IApiResponse<Response.SearchPartResponse?>> SearchPartsAsync(string partNumber)
        {
            var request = new Request.SearchPartRequest { PartNumber = partNumber };
            return SearchPartsAsync(request);
        }

        /// <summary>
        /// Search parts
        /// </summary>
        /// <param name="request">The search request</param>
        public async Task<IApiResponse<Response.SearchPartResponse?>> SearchPartsAsync(Request.SearchPartRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = new Uri(_configuration.Endpoint, Endpoints.Search.GetDescription());
            var response = await _httpClient.Value.PostAsync(endpoint, data);
            var responseString = await response.Content.ReadAsStringAsync();
            var instance = JsonConvert.DeserializeObject<Model.ServiceResult<Response.SearchPartResponse>>(responseString);

            var errors = new List<string>();
            errors.AddRange(ProcessThrottleResponse(response, responseString, out var retryIn));
            if (instance?.Errors?.Any() == true)
                errors.AddRange(instance.Errors);

            return new ApiResponse<Response.SearchPartResponse?>
            {
                Response = instance.Response,
                IsSuccessful = response.IsSuccessStatusCode,
                IsRequestThrottled = response.StatusCode == System.Net.HttpStatusCode.TooManyRequests,
                RetryIn = TimeSpan.FromSeconds(retryIn),
                StatusCode = (int)response.StatusCode,
                Errors = instance.Errors ?? new List<string>()
            };
        }

        /// <summary>
        /// Get information on a part
        /// </summary>
        /// <param name="partNumber">The part number to search</param>
        public Task<IApiResponse<Model.PartResults?>> GetPartInformationAsync(string partNumber)
        {
            var request = new Request.PartInformationRequest { PartNumber = partNumber };
            return GetPartInformationAsync(request);
        }

        /// <summary>
        /// Get information on a part
        /// </summary>
        /// <param name="request">The search request</param>
        public async Task<IApiResponse<Model.PartResults?>> GetPartInformationAsync(Request.PartInformationRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = new Uri(_configuration.Endpoint, Endpoints.Info.GetDescription());
            var response = await _httpClient.Value.PostAsync(endpoint, data);
            var responseString = await response.Content.ReadAsStringAsync();
            var instance = JsonConvert.DeserializeObject<Model.ServiceResult<Model.PartResults>>(responseString);

            var errors = new List<string>();
            errors.AddRange(ProcessThrottleResponse(response, responseString, out var retryIn));
            if (instance?.Errors?.Any() == true)
                errors.AddRange(instance.Errors);

            return new ApiResponse<Model.PartResults?>
            {
                Response = instance.Response,
                IsSuccessful = response.IsSuccessStatusCode,
                IsRequestThrottled = response.StatusCode == System.Net.HttpStatusCode.TooManyRequests,
                RetryIn = TimeSpan.FromSeconds(retryIn),
                StatusCode = (int)response.StatusCode,
                Errors = errors
            };
        }

        private List<string> ProcessThrottleResponse(HttpResponseMessage response, string responseString, out int retryIn)
        {
            retryIn = 0;
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                var throttleResponse = JsonConvert.DeserializeObject<ThrottleResponse>(responseString);
                if (response.Headers.TryGetValues("Retry-After", out var values))
                {
                    var retryInStr = values.First();
                    int.TryParse(retryInStr, out retryIn);
                }
                return new List<string>()
                {
                    $"{throttleResponse.Message} {throttleResponse.Details}"
                };
            }
            return new List<string>();
        }

        private static HttpClient CreateHttpClient()
        {
            // recommended practice for using HttpClient outside of dependency injection
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
            };
            return new HttpClient(handler);
        }
    }
}