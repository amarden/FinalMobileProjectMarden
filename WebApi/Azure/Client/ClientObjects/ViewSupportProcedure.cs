namespace Web.ClientObjects
{
    /// <summary>
    /// Class that represents a procedure that is done by a Nurse, used to populate the Nurse Home Page of their relevant procedures. 
    /// </summary>
    public class ViewSupportProcedure
    {
        public int PatientProcedureId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string ProcedureName { get; set; }
    }
}