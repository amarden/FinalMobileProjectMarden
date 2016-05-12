using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client.ClientObjects
{
    /// <summary>
    /// Represents PatientProcedure table in SQL
    /// </summary>
    public class PatientProcedure
    {
        public int PatientProcedureId { get; set; }
        public int PatientId { get; set; }
        public int ProcedureCodeId { get; set; }
        public DateTime AssignedTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public bool Completed { get; set; }
    }
}
