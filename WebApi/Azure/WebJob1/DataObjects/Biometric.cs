using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.DataObjects
{
    public class Biometric
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BiometricId { get; set; }
        public int PatientId { get; set; }
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public int Oxygen { get; set; }
        public int Glucose { get; set; }
        public int DeathModifier { get; set; }
        public DateTime MeasurementDate { get; set; }
        [JsonIgnore]
        public virtual Patient Patient { get; set; }
    }
}
