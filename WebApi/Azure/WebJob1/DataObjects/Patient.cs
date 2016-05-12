using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.DataObjects
{
    public class Patient
    {
        public Patient()
        {
            Biometrics = new HashSet<Biometric>();
            PatientProcedures = new HashSet<PatientProcedure>();
            PatientChatLogs = new HashSet<PatientChatLog>();
            PatientImagings = new HashSet<PatientImaging>();
            PatientProviders = new HashSet<PatientProvider>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientId { get; set; }
        public int DiagnosisCodeId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MedicalStatus { get; set; }
        public DateTime AdmitDate { get; set; }
        public DateTime? AssignDate{ get; set; }
        public DateTime? DischargeDate { get; set; }
        public virtual ICollection<Biometric> Biometrics { get; set; }
        public virtual ICollection<PatientProcedure> PatientProcedures { get; set; }
        public virtual ICollection<PatientChatLog> PatientChatLogs { get; set; }
        public virtual ICollection<PatientImaging> PatientImagings { get; set; }
        public virtual ICollection<PatientProvider> PatientProviders { get; set; }
        public virtual DiagnosisCode DiagnosisCode { get; set; }

    }
}
