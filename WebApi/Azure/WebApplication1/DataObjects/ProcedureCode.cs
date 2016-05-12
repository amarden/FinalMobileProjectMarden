using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azure.DataObjects
{
    /// <summary>
    /// Represents ProcedureCode table in SQL
    /// </summary>
    public class ProcedureCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProcedureCodeId { get; set; }
        public string Procedure { get; set; }
        public string Role { get; set; }
    }
}
