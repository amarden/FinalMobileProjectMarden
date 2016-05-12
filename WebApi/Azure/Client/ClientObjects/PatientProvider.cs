using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client.ClientObjects
{
    /// <summary>
    /// Represents PatientProvider table in SQL
    /// </summary>
    public class PatientProvider
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProviderPatientId { get; set; }
        public int PatientId { get; set; }
        public int ProviderId { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool Active { get; set; }
    }
}
