using Binner.SwarmApi.Request;
using Binner.SwarmApi.Response;

namespace Binner.SwarmApi
{
    /// <summary>
    /// Swarm Api
    /// </summary>
    public interface ISwarmApiClient
    {
        /// <summary>
        /// Search parts
        /// </summary>
        /// <param name="request">The search request</param>
        /// <returns>SearchPartResponse</returns>
        Task<IApiResponse<Response.SearchPartResponse?>> SearchPartsAsync(SearchPartRequest request);

        /// <summary>
        /// Search parts
        /// </summary>
        /// <param name="partNumber">The part number to search</param>
        /// <returns>SearchPartResponse</returns>
        Task<IApiResponse<Response.SearchPartResponse?>> SearchPartsAsync(string partNumber);

        /// <summary>
        /// Get information on a part
        /// </summary>
        /// <param name="partNumber">The part number to search</param>
        Task<IApiResponse<Model.PartResults?>> GetPartInformationAsync(string partNumber);

        /// <summary>
        /// Get information on a part
        /// </summary>
        /// <param name="request">The search request</param>
        Task<IApiResponse<Model.PartResults?>> GetPartInformationAsync(PartInformationRequest request);
    }
}