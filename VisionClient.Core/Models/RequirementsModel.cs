namespace VisionClient.Core.Models
{
    public class RequirementsModel
    {
        public RequirementsModel()
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

        public string MinimumOS { get; set; }
        public string MinimumMemory { get; set; }
        public string MinimumCPU { get; set; }
        public string MinimumGPU { get; set; }
        public string MinimumStorage { get; set; }
        public string RecommendedOS { get; set; }
        public string RecommendedMemory { get; set; }
        public string RecommendedCPU { get; set; }
        public string RecommendedGPU { get; set; }
        public string RecommendedStorage { get; set; }

    }
}
