namespace Binner.SwarmApi
{
    public interface IApiResponse<T>
    {
        /// <summary>
        /// List of errors if any
        /// </summary>
        IEnumerable<string> Errors { get; set; }

        /// <summary>
        /// True if the request is throttled
        /// </summary>
        bool IsRequestThrottled { get; set; }

        /// <summary>
        /// True if the request was successful
        /// </summary>
        bool IsSuccessful { get; set; }

        /// <summary>
        /// Response data
        /// </summary>
        T? Response { get; set; }

        /// <summary>
        /// If the request is throttled, the amount of time to retry request in
        /// </summary>
        TimeSpan RetryIn { get; set; }

        /// <summary>
        /// The http status code of the response
        /// </summary>
        int StatusCode { get; set; }
    }
}