using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.DashboardObjects
{
    /// <summary>
    /// Represents small amount of patient information to attach to the providers that serve them in ProviderDashboard class
    /// </summary>
    public class PatientSimple
    {
        public string diagnosis { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
    }
}