using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client.ClientObjects
{
    /// <summary>
    /// Class which represents the Biometric table in SQL
    /// </summary>
    public class Biometric
    {
        public int BiometricId { get; set; }
        public int PatientId { get; set; }
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public int Oxygen { get; set; }
        public int Glucose { get; set; }
        public string BloodPressure { get { return Systolic + "/" + Diastolic;  } }
        public string OPercent { get { return Oxygen+"%"; } }

        public DateTime MeasurementDate { get; set; }
    }
}
