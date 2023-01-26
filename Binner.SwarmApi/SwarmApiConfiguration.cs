namespace Binner.SwarmApi
{
    /// <summary>
    /// Swarm api configuration
    /// </summary>
    public class SwarmApiConfiguration
    {
        /// <summary>
        /// The optional api key used to access the api
        /// </summary>
        /// <remarks>To obtain an api key visit https://binner.io/swarm and create an account.</remarks>
        public string? ApiKey { get; }

        /// <summary>
        /// The url address to the Swarm api.
        /// Default: https://swarm.binner.io
        /// </summary>
        public Uri Endpoint { get; } = new Uri("https://swarm.binner.io");

        /// <summary>
        /// Create a Swarm api configuration
        /// </summary>
        public SwarmApiConfiguration() { }

        /// <summary>
        /// Create a Swarm api configuration
        /// </summary>
        /// <param name="apiKey">Your private api key</param>
        public SwarmApiConfiguration(string apiKey)
        {
            ApiKey = apiKey;
        }

        /// <summary>
        /// Create a Swarm api configuration
        /// </summary>
        /// <param name="apiKey">Your private api key</param>
        /// <param name="apiEndpoint">The url address to the api</param>
        public SwarmApiConfiguration(string apiKey, Uri apiEndpoint) : this(apiKey)
        {
            Endpoint = apiEndpoint;
        }
    }
}
