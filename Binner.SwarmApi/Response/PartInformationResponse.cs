using Binner.SwarmApi.Model;

namespace Binner.SwarmApi.Response
{
    public class PartInformationResponse
    {
        /// <summary>
        /// Part information associated with searched keywords
        /// </summary>
        public ICollection<PartNumber> Parts { get; set; } = new List<PartNumber>();
    }
}
