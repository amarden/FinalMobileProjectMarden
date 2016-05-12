using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.DataObjects
{
    /// <summary>
    /// Represents Provider table in SQL
    /// </summary>
    public class Provider
    {
        public Provider()
        {
            PatientProviders = new HashSet<PatientProvider>();
            PatientProcedures = new HashSet<PatientProcedure>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProviderId { get; set; }
        public string Name { get; set; }
        public string TwitterUserId { get; set; }
        public string Role { get; set; }
        public virtual ICollection<PatientProvider> PatientProviders { get; set; }
        public virtual ICollection<PatientProcedure> PatientProcedures { get; set; }

    }
}
