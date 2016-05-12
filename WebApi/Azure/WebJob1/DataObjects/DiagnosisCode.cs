using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.DataObjects
{
    public class DiagnosisCode
    {
        public DiagnosisCode()
        {
            Patients = new HashSet<Patient>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiagnosisCodeId { get; set; }
        public string Diagnosis { get; set; }
        [JsonIgnore]
        public ICollection<Patient> Patients { get; set; }
    }
}
