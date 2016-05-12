using Azure.DataObjects;
using System;
using System.Collections.Generic;

namespace Azure.ClientObjects
{
    /// <summary>
    /// Class that represents a Patient that is used when listing out patients in teh client application
    /// </summary>
    public class ViewPatient
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string MedicalStatus { get; set; }
        public string Diagnosis { get; set; }
        public DateTime AdmitDate { get; set; }
        public DateTime? DischargeDate{ get; set; }
        public int NumProvidersAssigned{ get; set; }
    }
}