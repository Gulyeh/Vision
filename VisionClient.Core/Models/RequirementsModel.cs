using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Models
{
    public class RequirementsModel
    {
        public RequirementsModel()
        {
            OsMinimum = string.Empty;
            MemoryMinimum = string.Empty;
            CpuMinimum = string.Empty;
            GpuMinimum = string.Empty;
            StorageMinimum = string.Empty;
            OsRecommended = string.Empty;
            MemoryRecommended = string.Empty;
            CpuRecommended = string.Empty;
            GpuRecommended = string.Empty;
            StorageRecommended = string.Empty;
        }

        public string OsMinimum { get; set; }
        public string MemoryMinimum { get; set; }
        public string CpuMinimum { get; set; }
        public string GpuMinimum { get; set; }
        public string StorageMinimum { get; set; }
        public string OsRecommended { get; set; }
        public string MemoryRecommended { get; set; }
        public string CpuRecommended { get; set; }
        public string GpuRecommended { get; set; }
        public string StorageRecommended { get; set; }

    }
}
