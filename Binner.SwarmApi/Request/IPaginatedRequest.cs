namespace Binner.SwarmApi.Request
{
    public interface IPaginatedRequest
    {
        /// <summary>
        /// Total records to return
        /// </summary>
        int RecordCount { get; set; }
    }
}
