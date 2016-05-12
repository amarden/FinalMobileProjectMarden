using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client.ClientObjects
{
    /// <summary>
    /// Represents Patient data that is shown as a list to assigned providers
    /// </summary>
    public class Patient
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string MedicalStatus { get; set; }
        public string Diagnosis { get; set; }
        public DateTime AdmitDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public int NumProvidersAssigned { get; set; }
    }
}
