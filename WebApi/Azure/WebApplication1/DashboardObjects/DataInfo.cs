using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.DashboardObjects
{
    /// <summary>
    /// Class that contains other objects that represents all the data used for the dashboard web app
    /// </summary>
    public class DataInfo
    {
        public IEnumerable<PatientDashboard> patients { get; set; }
        public IEnumerable<ProviderDashboard> providers { get; set; }
        public IEnumerable<Item> procedures { get; set; }
        public IEnumerable<Item> diagnoses { get; set; }
    }
}