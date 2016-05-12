using Azure.DataObjects;
using Azure.Models;
using Faker;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.EhrAssets
{
    /// <summary>
    /// A class that tries to mock certain aspects of an EHR system. Primarily used for two things. 
    /// One, to create new patients, and Two to generate fake biometrics for these patients
    /// </summary>
    public class EHR
    {
        private List<DiagnosisCode> dxCodes = new List<DiagnosisCode>();
        private List<Provider> administrators = new List<Provider>();
        //Stable ranges and critical ranges to help calculate medical status
        private Tuple<int, int> systolicStableRange = new Tuple<int, int>(80, 145);
        private Tuple<int, int> diastolicStableRange = new Tuple<int, int>(50, 90);
        private Tuple<int, int> glucoseStableRange = new Tuple<int, int>(50, 110);
        private Tuple<int, int> oxygenStableRange = new Tuple<int, int>(85, 100);
        private Tuple<int, int> systolicCriticalBounds = new Tuple<int, int>(60, 195);
        private Tuple<int, int> diastolicCrticalBounds = new Tuple<int, int>(20, 120);
        private Tuple<int, int> glucoseCriticalBounds = new Tuple<int, int>(20, 160);
        private int oxygenCriticalLowerBound = 65;
        //Random number generator used in several methods to generate fake data
        Random r = new Random();

        /// <summary>
        /// Constructr that populates who the administrator users are and what the possible diagnoses are
        /// </summary>
        public EHR()
        {
            using (var db = new DataContext())
            {
                this.dxCodes = db.DiagnosisCodes.ToList();
                this.administrators = db.Providers.Where(x => x.Role == "Administrator").ToList();
            }
        }

        //Overloaded constructor used for testing
        public EHR(bool test)
        {
        }

        /// <summary>
        /// Method that creates the numbers of new patients based on the number
        /// </summary>
        /// <param name="number"></param>
        public void CreateNewPatients(int number)
        {
            List<Patient> patientsToAdd = new List<Patient>();
            for(int i=0; i< number; i++)
            {
                //Generate some specfic information about the patient
                var p = new Patient();
                p.Gender = generateGender();
                p.Name = generateName(p.Gender);
                p.Age = generateAge();
                p.AdmitDate = DateTime.Now;
                p.DiagnosisCodeId = generateDiagnosis();
                //Create the first biometrics of the user
                var firstMetrics = initialValues();
                PatientProvider assignedAdministrator = assignToRandomAdministrator();
                //Based on biometrics we want to impute the patient satus and their deathmodifider used to calculate when they die or are critical
                var statusAndScore = imputeStatus(p.Biometrics, firstMetrics);
                firstMetrics.DeathModifier = statusAndScore.Item2;
                p.MedicalStatus = statusAndScore.Item1;
                p.Biometrics.Add(firstMetrics);
                p.PatientProviders.Add(assignedAdministrator);
                patientsToAdd.Add(p);
            }
            using (var db = new DataContext())
            {
                db.Patients.AddRange(patientsToAdd);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// When a patient is created randomly assigns them to an existing administrator
        /// </summary>
        /// <returns></returns>
        public PatientProvider assignToRandomAdministrator()
        {
            var pp = new PatientProvider();
            var administratorCount = this.administrators.Count;
            var assignTo = r.Next(0, administratorCount);
            var admin = this.administrators[assignTo];
            pp.ProviderId = admin.ProviderId;
            pp.AssignedDate = DateTime.Now;
            pp.Active = true;
            return pp;
        }

        /// <summary>
        /// Randomly creates initial values for biometrics
        /// </summary>
        /// <returns></returns>
        public Biometric initialValues()
        {
            Biometric b = new Biometric();
            b.Systolic = r.Next(80, 160);
            b.Diastolic = r.Next(50, 90);
            b.Oxygen = r.Next(75, 100);
            b.Glucose = r.Next(20, 160);
            b.MeasurementDate = DateTime.Now;
            return b;
        }

        /// <summary>
        /// calcualtes the death modifier used to determine a patient's status
        /// </summary>
        /// <param name="lengthOfStay"></param>
        /// <param name="measure"></param>
        /// <returns>integer</returns>
        public int getDeathModifierCalculated(int lengthOfStay, Biometric measure)
        {
            //status for each measurement can be stable, unstable, or critical
            string bpStatus = checkBpStatus(measure.Systolic, measure.Diastolic);
            string glucoseStatus = checkGlucoseStatus(measure.Glucose);
            string oxygenStatus = checkOxygenStatus(measure.Oxygen);


            //Calculate deathModifier that raises likelihood of patient death or critical status
            List<string> statuses = new List<string> { bpStatus, glucoseStatus, oxygenStatus };
            int criticalCount = statuses.Where(x => x == "critical").Count();
            int unstableCount = statuses.Where(x => x == "unstable").Count();
            int stableCount = statuses.Where(x => x == "stable").Count();

            return lengthOfStay + (criticalCount * 10) + (unstableCount * 2) + (-stableCount * 2);
        }

        /// <summary>
        /// takes current biometric measure and all past biometric measures determines what the medicalstatus of a patient is
        /// </summary>
        /// <param name="allMeasures"></param>
        /// <param name="measure"></param>
        /// <returns>Tuple of medical status and death modifier</returns>
        public Tuple<string, int> imputeStatus(IEnumerable<Biometric> allMeasures, Biometric measure)
        {
            int lengthOfStay = allMeasures.Count();
            int averageDeathModifier = lengthOfStay == 0 ? 0 : (int)Math.Round(allMeasures.Average(x => x.DeathModifier)/10);

            int minRandomModifider = -5 + averageDeathModifier;
            int pastDeathModifier = r.Next(minRandomModifider, 10);

            int deathModifier = getDeathModifierCalculated(lengthOfStay, measure) + pastDeathModifier;
            int chanceOfDeath = lengthOfStay == 0 ? 0 : deathModifier; //Cannot be dead on first measurement
            int randomInt = r.Next(0, 100);
            string patientStatus;
            if (randomInt < chanceOfDeath && chanceOfDeath > 0)
            {
                patientStatus = "death";
            }
            else if(randomInt - chanceOfDeath < 10)
            {
                patientStatus = "critical";
            }
            else
            {
                patientStatus = "stable";
            }
            return new Tuple<string, int>(patientStatus, deathModifier);

        }

        /// <summary>
        /// Takes oxygen number and determines whether the range is stable, unstable, or critical
        /// </summary>
        /// <param name="oxygen"></param>
        /// <returns>metric status</returns>
        public string checkOxygenStatus(int oxygen)
        {
            string status;
            if (this.oxygenStableRange.Item1 <= oxygen && this.oxygenStableRange.Item2 >= oxygen)
            {
                status = "stable";
            }
            else if (oxygen < this.oxygenCriticalLowerBound)
            {
                status = "critical";
            }
            else
            {
                status = "unstable";
            }
            return status;
        }


        /// <summary>
        /// Takes glucose number and determines whether the range is stable, unstable, or critical
        /// </summary>
        /// <param name="glucose"></param>
        /// <returns>metric status</returns>
        public string checkGlucoseStatus(int glucose)
        {
            string status;
            if (this.glucoseStableRange.Item1 <= glucose && this.glucoseStableRange.Item2 >= glucose)
            {
                status = "stable";
            }
            else if (glucose > this.glucoseCriticalBounds.Item2 || glucose < this.glucoseCriticalBounds.Item1)
            {
                status = "critical";
            }
            else
            {
                status = "unstable";
            }
            return status;
        }

        /// <summary>
        /// Takes BP numbers and determines whether the range is stable, unstable, or critical
        /// </summary>
        /// <param name="systolic"></param>
        /// <param name="diastolic"></param>
        /// <returns>metric status</returns>
        public string checkBpStatus(int systolic, int diastolic)
        {
            string status;
            if(this.systolicStableRange.Item1 <= systolic && this.systolicStableRange.Item2 >= systolic
                && this.diastolicStableRange.Item1 <= diastolic && this.diastolicStableRange.Item2 >= diastolic)
            {
                status = "stable";
            }
            else if(systolic > this.systolicCriticalBounds.Item2 || systolic < this.systolicCriticalBounds.Item1
                || diastolic > this.diastolicCrticalBounds.Item2 || diastolic < this.diastolicCrticalBounds.Item1)
            {
                status = "critical";
            }
            else
            {
                status = "unstable";
            }
            return status;
        }

        /// <summary>
        /// Randomly chooses a diagnosis
        /// </summary>
        /// <returns>integer representing diagnosis chosen</returns>
        public int generateDiagnosis()
        {
            return r.Next(1, 100);
        }

        /// <summary>
        /// Randomly generates an age
        /// </summary>
        /// <returns></returns>
        public int generateAge()
        {
            return r.Next(0, 100);
        }


        /// <summary>
        /// Randomly generates the patient's gender
        /// </summary>
        /// <returns></returns>
        public string generateGender()
        {
            int rInt = r.Next(1, 3);
            string gender = rInt == 1 ? "male" : "female";
            return gender;
        }


        /// <summary>
        /// Randomly generates a patients name based on gener
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public string generateName(string gender)
        {
            string name="";
            if(gender == "male")
            {
                name = NameFaker.MaleFirstName() + " " + NameFaker.LastName();
            }
            else if(gender == "female")
            {
                name = NameFaker.FemaleFirstName() + " " + NameFaker.LastName();
            }
            return name;
        }
    }
}
