namespace Client.ClientObjects
{
    /// <summary>
    /// Represents ProcedureCode table in SQL
    /// </summary>
    public class ProcedureCode
    {
        public int ProcedureCodeId { get; set; }
        public string Procedure { get; set; }
        public string Role { get; set; }
    }
}
