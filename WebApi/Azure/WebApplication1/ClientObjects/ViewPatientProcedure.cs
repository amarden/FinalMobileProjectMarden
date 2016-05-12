using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azure.ClientObjects
{
    /// <summary>
    /// Class that represents a single procedure that is assigned to a patient, used in ProcedurePage on client application
    /// </summary>
    public class ViewPatientProcedure
    {

        public ViewPatientProcedure()
        {
            this.ShowRules = new VisibilityRules();
        }
        public int PatientProcedureId { get; set; }
        public string Procedure { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedTime { get; set; }
        public string CompletedBy { get; set; }
        public string userRole { get; set; }
        public string procedureRole { get; set; }
        public VisibilityRules ShowRules { get; set; }
    }

    /// <summary>
    /// Class used in ViewPatientProcedure that is used to determine when the procedure can be interacted with and by whom.
    /// </summary>
    public class VisibilityRules
    {
        public bool Completed { get; set; }
        public string userRole { get; set; }
        public string procedureRole { get; set; }
    }
}