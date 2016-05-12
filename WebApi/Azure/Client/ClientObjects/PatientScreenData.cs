using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.ClientObjects;

namespace Client.ClientObjects
{
    public class PatientScreenData
    {
        public User User { get; set; }
        public PatientDetail Patient { get; set; }
        public AuthInfo auth { get; set; }
    }

    public class AuthInfo
    {
        public string userid { get; set; }
        public string token { get; set; }
    }
}
