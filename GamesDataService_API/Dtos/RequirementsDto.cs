using System.ComponentModel.DataAnnotations;

namespace GamesDataService_API.Dtos
{
    public class RequirementsDto
    {
        public RequirementsDto()
        {
            MinimumOS = string.Empty;
            MinimumMemory = string.Empty;
            MinimumCPU = string.Empty;
            MinimumGPU = string.Empty;
            MinimumStorage = string.Empty;
            RecommendedOS = string.Empty;
            RecommendedMemory = string.Empty;
            RecommendedCPU = string.Empty;
            RecommendedGPU = string.Empty;
            RecommendedStorage = string.Empty;
        }

        [Required]
        public string MinimumOS { get; set; }
        [Required]
        public string MinimumMemory { get; set; }
        [Required]
        public string MinimumCPU { get; set; }
        [Required]
        public string MinimumGPU { get; set; }
        [Required]
        public string MinimumStorage { get; set; }
        [Required]
        public string RecommendedOS { get; set; }
        [Required]
        public string RecommendedMemory { get; set; }
        [Required]
        public string RecommendedCPU { get; set; }
        [Required]
        public string RecommendedGPU { get; set; }
        [Required]
        public string RecommendedStorage { get; set; }
    }
}