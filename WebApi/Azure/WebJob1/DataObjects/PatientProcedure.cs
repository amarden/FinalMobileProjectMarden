using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.DataObjects
{
    public class PatientProcedure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientProcedureId { get; set; }
        public int PatientId { get; set; }
        public int ProcedureCodeId{ get; set; }
        public DateTime AssignedTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public bool Completed { get; set; }
        public int? ProviderId { get; set; }
        [JsonIgnore]
        public virtual Patient Patient { get; set; }
        [JsonIgnore]
        public virtual Provider Provider { get; set; }
        public virtual ProcedureCode ProcedureCode { get; set; }
    }
}
