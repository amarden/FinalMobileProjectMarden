using Azure.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.DashboardObjects
{
    /// <summary>
    /// Represents a single provider with information used for the dashboard
    /// </summary>
    public class ProviderDashboard
    {
        public int ProviderId { get; set; }
        public string Role { get; set; }
        public IEnumerable<PatientSimple> patients { get; set; }
        public IEnumerable<string> procedures { get; set; }
    }
}