using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azure.ClientObjects
{
    /// <summary>
    /// Class that represents the assignment of a provider to a patient used on the client application on the ProviderPage
    /// </summary>
    public class ViewPatientProvider
    {
        public int ProviderPatientId { get; set; }
        public int ProviderId { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
    }
}