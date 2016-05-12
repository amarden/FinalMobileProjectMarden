using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client.ClientObjects
{
    /// <summary>
    /// Represents a Chat message 
    /// </summary>
    public class PatientChatLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientChatLogId { get; set; }
        public int PatientId { get; set; }
        public int ProviderId { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
    }
}
