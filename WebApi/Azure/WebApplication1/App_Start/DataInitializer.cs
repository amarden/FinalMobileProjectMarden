using Azure.DataObjects;
using Azure.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Web;

namespace Azure.App_Start
{
    /// <summary>
    /// Class that is invoked at appstart and run when the database model has changed compared to the code first implementation of the database
    /// </summary>
    public class DataInitializer : DropCreateDatabaseIfModelChanges<DataContext>
    {
        /// <summary>
        /// Main method called when this class is invoked, takes our DbContext class. The class does 4 things: first three are populate the database with diagnoses, procedures, and providers
        /// The second is to create a view in order to be used by Azure Search
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(DataContext context)
        {
            List<DiagnosisCode> diagnoses = getDiagnoses();
            List<ProcedureCode> procedures = getProcedures();
            List<Provider> providers = getProviders();
            createSearchView();
            using (var db = new DataContext())
            {
                db.DiagnosisCodes.AddRange(diagnoses);
                db.ProcedureCodes.AddRange(procedures);
                db.Providers.AddRange(providers);
                db.SaveChanges();
            }

            base.Seed(context);
        }

        /// <summary>
        /// Creates provider from data based on our comma seperated text file in StartingData Folder called ProviderStartList.txt
        /// </summary>
        /// <returns>List of Providers to populate our providers table</returns>
        public List<Provider> getProviders()
        {
            var providers = new List<Provider>();
            string path = HttpContext.Current.Server.MapPath("~/StartingData/ProviderStartList.txt");
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(',');
                    var provider = new Provider();
                    provider.Name = data[0];
                    provider.Role = data[1];
                    providers.Add(provider);
                }
            }
            return providers;
        }

        /// <summary>
        /// Creates diagnoses from data based on our comma seperated text file in StartingData Folder called DiagnosisCode.txt
        /// </summary>
        /// <returns>List of DiagnoseCodes to populate our diagnosis table</returns>
        public List<DiagnosisCode> getDiagnoses()
        {
            var codes = new List<DiagnosisCode>();
            string path = HttpContext.Current.Server.MapPath("~/StartingData/DiagnosisCodes.txt");
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var code = new DiagnosisCode();
                    code.Diagnosis = line;
                    codes.Add(code);
                }
            }
            return codes;
        }

        /// <summary>
        /// Creates procedures from data based on our comma seperated text file in StartingData Folder called ProcedureCodes.txt
        /// </summary>
        /// <returns>List of Procedures to populate our ProcedureCode table</returns>
        public List<ProcedureCode> getProcedures()
        {
            var codes = new List<ProcedureCode>();
            string path = HttpContext.Current.Server.MapPath("~/StartingData/ProcedureCodes.txt");
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] items = line.Split(',');
                    var code = new ProcedureCode();
                    code.Procedure = items[0];
                    code.Role = items[1];
                    codes.Add(code);
                }
            }
            return codes;
        }

        /// <summary>
        /// Executes SQL code that creates a view in our database which is used by Azure search
        /// </summary>
        private void createSearchView()
        {
            string viewCreator = "CREATE VIEW [dbo].[SearchView] " +
                        "AS " +
                        "SELECT row_number() OVER(ORDER BY Name) as id, dbo.Patients.Name, dbo.Patients.Age, dbo.Patients.Gender, dbo.Patients.MedicalStatus, dbo.PatientProviders.ProviderId, dbo.DiagnosisCodes.Diagnosis " +
                        "FROM dbo.Patients INNER JOIN " +
             " dbo.PatientProviders ON dbo.Patients.PatientId = dbo.PatientProviders.PatientId INNER JOIN " +
             " dbo.DiagnosisCodes ON dbo.Patients.DiagnosisCodeId = dbo.DiagnosisCodes.DiagnosisCodeId";


            using (var db = new DataContext())
            {
                db.Database.ExecuteSqlCommand(viewCreator);
            }
        }
    }
}
