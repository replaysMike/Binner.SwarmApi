using System.ComponentModel.DataAnnotations;

namespace Binner.SwarmApi.Request
{
    /// <summary>
    /// Search part request
    /// </summary>
    public class SearchPartRequest
    {
        /// <summary>
        /// Part number
        /// </summary>
        [Required]
        public string PartNumber { get; set; } = null!;

        /// <summary>
        /// Type of part
        /// </summary>
        public string? PartType { get; set; }

        /// <summary>
        /// Mounting type
        /// </summary>
        public string? MountingType { get; set; }
    }
}
