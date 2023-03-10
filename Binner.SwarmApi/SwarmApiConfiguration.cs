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
        /// Swarm api connect timeout. Default: 5 seconds
        /// </summary>
        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Swarm api request timeout. Default: 5 seconds
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Gets or sets how long a connection can be in the pool to be considered reusable. Default: 15 minutes
        /// </summary>
        public TimeSpan PooledConnectionLifetime { get; set; } = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Gets or sets the timespan to wait for data to be drained from responses. Default: 10 seconds
        /// </summary>
        public TimeSpan ResponseDrainTimeout { get; set; } = TimeSpan.FromSeconds(10);

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
