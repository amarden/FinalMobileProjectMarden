using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.AuthenticationHelpers
{
    /// <summary>
    /// Class used to catch information from authentication providers such as twitter and google. Used in credentials Class
    /// </summary>
    public class UserSubmit
    {
        public string Provider { get; set; }
        [JsonProperty(PropertyName = "Id")]
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string PictureUrl { get; set; }
        public string Role { get; set; }
    }
}
