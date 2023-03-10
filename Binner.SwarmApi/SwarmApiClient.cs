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
        private readonly string ApiKeyHeaderName = "X-ApiKey";
        private readonly SwarmApiConfiguration _configuration = new();
        private readonly Lazy<HttpClient> _httpClient;
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
            _httpClient = new Lazy<HttpClient>(() => CreateHttpClient(_configuration));
        }

        /// <summary>
        /// Create a Swarm api client
        /// <see cref="https://swarm.binner.io"/>
        /// </summary>
        /// <param name="configuration">Configuration to apply</param>
        public SwarmApiClient(SwarmApiConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new Lazy<HttpClient>(() => CreateHttpClient(configuration));
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
            var endpoint = new Uri(_configuration.Endpoint, Endpoints.Search.GetDescription());
            var content = CreateSerializedRequest(request);

            try
            {
                var response = await _httpClient.Value.PostAsync(endpoint, content);
                var (instance, retryIn, errors) =
                    await GetResponseAsync<Model.ServiceResult<Response.SearchPartResponse>>(response);
                if (instance?.Errors?.Any() == true)
                    errors.AddRange(instance.Errors);

                return new ApiResponse<Response.SearchPartResponse?>
                {
                    Response = instance?.Response,
                    IsSuccessful = response.IsSuccessStatusCode,
                    IsRequestThrottled = response.StatusCode == System.Net.HttpStatusCode.TooManyRequests,
                    RetryIn = TimeSpan.FromSeconds(retryIn),
                    StatusCode = (int)response.StatusCode,
                    Errors = errors
                };
            }
            catch (TaskCanceledException ex)
            {
                throw new TimeoutException($"Api request timeout ({_configuration.Timeout}) exceeded!", ex.GetBaseException());
            }
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
            var endpoint = new Uri(_configuration.Endpoint, Endpoints.Info.GetDescription());
            var content = CreateSerializedRequest(request);
            try
            {
                var response = await _httpClient.Value.PostAsync(endpoint, content);
                var (instance, retryIn, errors) =
                    await GetResponseAsync<Model.ServiceResult<Model.PartResults>>(response);
                if (instance?.Errors?.Any() == true)
                    errors.AddRange(instance.Errors);
                return new ApiResponse<Model.PartResults?>
                {
                    Response = instance?.Response,
                    IsSuccessful = response.IsSuccessStatusCode,
                    IsRequestThrottled = response.StatusCode == System.Net.HttpStatusCode.TooManyRequests,
                    RetryIn = TimeSpan.FromSeconds(retryIn),
                    StatusCode = (int)response.StatusCode,
                    Errors = errors
                };
            }
            catch (TaskCanceledException ex)
            {
                throw new TimeoutException($"Api request timeout ({_configuration.Timeout}) exceeded!", ex.GetBaseException());
            }
        }

        /// <summary>
        /// Get the response object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<(T? instance, int retryIn, List<string> errors)> GetResponseAsync<T>(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var instance = JsonConvert.DeserializeObject<T>(responseString);
            var errors = new List<string>();
            if (instance == null)
                errors.Add($"Unknown response received! '{responseString}'");
            errors.AddRange(ProcessThrottleResponse(response, responseString, out var retryIn));
            return (instance, retryIn, errors);
        }

        /// <summary>
        /// Create a content object to issue the request with
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private StringContent CreateSerializedRequest(object request)
        {
            var json = JsonConvert.SerializeObject(request);
            var jsonString = new StringContent(json, Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(_configuration.ApiKey))
                jsonString.Headers.Add(ApiKeyHeaderName, _configuration.ApiKey);
            return jsonString;
        }

        /// <summary>
        /// Check the response for a 429 status code. If found parse the throttle response
        /// </summary>
        /// <param name="response"></param>
        /// <param name="responseString"></param>
        /// <param name="retryIn"></param>
        /// <returns></returns>
        private List<string> ProcessThrottleResponse(HttpResponseMessage response, string responseString, out int retryIn)
        {
            retryIn = 0;
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                if (response.Headers.TryGetValues("Retry-After", out var values))
                {
                    var retryInStr = values.First();
                    int.TryParse(retryInStr, out retryIn);
                }

                var throttleResponse = JsonConvert.DeserializeObject<ThrottleResponse>(responseString);
                if (throttleResponse == null)
                    return new List<string> { $"Request was throttled, but received unknown response! '{responseString}'" };

                return new List<string>()
                {
                    $"{throttleResponse.Message} {throttleResponse.Details}"
                };
            }
            return new List<string>();
        }

        private static HttpClient CreateHttpClient(SwarmApiConfiguration configuration)
        {
            // recommended practice for using HttpClient outside of dependency injection
            var handler = new SocketsHttpHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                // Recreate every 15 minutes
                PooledConnectionLifetime = configuration.PooledConnectionLifetime,
                ConnectTimeout = configuration.ConnectTimeout,
                ResponseDrainTimeout = configuration.ResponseDrainTimeout,
            };
            var client = new HttpClient(handler);

            // set the request timeout
            client.Timeout = configuration.Timeout;

            return client;
        }
    }
}