using Azure.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.DashboardObjects
{
    /// <summary>
    /// Represents a single patient with information used for the dashboard
    /// </summary>
    public class PatientDashboard
    {
        public string PatientId { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public IEnumerable<string> procedures { get; set; }
        public string diagnosis { get; set; }
        public int providerCount { get; set; }
        public int imageCount { get; set; }
        public int chatCount { get; set; }
        public string MedicalStatus { get; set; }
    }
}