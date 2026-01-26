using System;

namespace BlazorApp1.Data
{
    public class MESProjectManagement
    {
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public string MoldCode { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string CustomerInfo { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public DateTime? InternalLeadTime { get; set; }
        public DateTime? CustomerLeadTime { get; set; }
        public string AccuracyLevel { get; set; } = string.Empty;
        public int? Priority { get; set; }
        public string ProjectStatus { get; set; } = string.Empty;
    }
}
