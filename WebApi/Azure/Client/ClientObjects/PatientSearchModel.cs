using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ClientObjects
{
    /// <summary>
    /// Class that represents the schema of what our azure search returns
    /// </summary>
    public class PatientSearchModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MedicalStatus { get; set; }
        public string Diagnosis { get; set; }
        public int ProviderId { get; set; }
        public bool match { get; set; }
    }
}
