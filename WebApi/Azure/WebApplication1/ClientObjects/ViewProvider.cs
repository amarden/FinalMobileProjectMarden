using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azure.ClientObjects
{
    /// <summary>
    /// Represents a provider used by client application
    /// </summary>
    public class ViewProvider
    {
        public int ProviderId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}