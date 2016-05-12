using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.DataObjects
{
    public class PatientChatLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientChatLogId { get; set; }
        public int PatientId { get; set; }
        public int ProviderId { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
        [JsonIgnore]
        public virtual Patient Patient { get; set; }
        [JsonIgnore]
        public virtual Provider Provider { get; set; }
    }
}
