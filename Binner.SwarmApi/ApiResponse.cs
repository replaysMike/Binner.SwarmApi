namespace Binner.SwarmApi
{
    public class ApiResponse<T> : IApiResponse<T>
    {
        /// <summary>
        /// Response data
        /// </summary>
        public T? Response { get; set; }

        /// <summary>
        /// True if the request was successful
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// True if the request is throttled
        /// </summary>
        public bool IsRequestThrottled { get; set; }

        /// <summary>
        /// If the request is throttled, the amount of time to retry request in
        /// </summary>
        public TimeSpan RetryIn { get; set; }

        /// <summary>
        /// The http status code of the response
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// List of errors if any
        /// </summary>
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}
