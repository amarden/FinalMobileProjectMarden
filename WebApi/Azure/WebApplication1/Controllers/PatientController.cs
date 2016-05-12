using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.AuthenticationHelpers;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.EhrAssets;
using Azure.Models;
using Azure.Temporary;
using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Azure.Controllers
{
    [Authorize]
    public class PatientController : ApiController
    {
        /// <summary>
        /// our dbcontext that represents our Azure SQL Database
        /// </summary>
        private DataContext db = new DataContext();

        /// <summary>
        /// Settings used to pass through to our authentication to get information from twitter auth api
        /// </summary>
        public MobileAppSettingsDictionary ConfigSettings => Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

        /// <summary>
        /// Class represents the number of patients to generate
        /// </summary>
        public class PatientCreate
        {
            public int number { get; set; }
        }

        /// <summary>
        /// Creates fake patients using the ehr service
        /// </summary>
        /// <param name="toCreate"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> CreatePatients(PatientCreate toCreate)
        {
            //Determines that the user is a superuser as they can only execute this service
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (provider.Role != "SuperUser")
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be an administrator to complete this task"
                });
            }
            // Our EHR service which includes information to create new fake patients
            EHR ehr = new EHR();
            ehr.CreateNewPatients(toCreate.number);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Queries a single patient from patient table
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns>ViewPatientDetail Object</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetPatient(int patientId)
        {
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (!cred.isAssigned(patientId))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be assigned to this patient to retrive this information"
                });
            }

            var config = new MapperConfiguration(cfg =>
             {
                 cfg.CreateMap<Patient, ViewPatientDetail>()
                    .ForMember(dto => dto.Diagnosis, conf => conf.MapFrom(x => x.DiagnosisCode.Diagnosis))
                    .ForMember(dto => dto.ProviderNumber, conf => conf.MapFrom(x => x.PatientProviders.Where(c => c.Active == true).Count()))
                    .ForMember(dto => dto.ChatActivityNumber, conf => conf.MapFrom(x => x.PatientChatLogs.Count()))
                    .ForMember(dto => dto.ImageNumber, conf => conf.MapFrom(x => x.PatientImagings.Count()))
                    .ForMember(dto => dto.ProcedureNumber, conf => conf.MapFrom(x => x.PatientProcedures.Where(c=>c.Completed == false).Count()));
             });

            var patient = db.Patients
                .Include("PatientToDos")
                .Include("PatientProviders")
                .Include("PatientProcedures")
                .Include("PatientChatLogs")
                .Where(x=>x.PatientId == patientId)
                .ProjectTo<ViewPatientDetail>(config).Single();

            return Request.CreateResponse(HttpStatusCode.OK, patient);
        }

        /// <summary>
        /// Returns a list of all patients assigned to the provider
        /// </summary>
        /// <returns>Collection of ViewPatients</returns>
        [HttpGet]
        public async Task<List<ViewPatient>> GetAssignedPatients()
        {
            //Gets user
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            //var providerId = provider.Id;
            var providerId = provider.Id;

            var config = new MapperConfiguration(cfg =>
             cfg.CreateMap<Patient, ViewPatient>()
               .ForMember(dto => dto.Diagnosis, conf => conf.MapFrom(x => x.DiagnosisCode.Diagnosis))
               .ForMember(dto => dto.NumProvidersAssigned, conf => conf.MapFrom(x=>x.PatientProviders.Where(c=>c.Provider.Role != "Administrator").Count()))
               );

            return db.PatientProviders
                .Where(x => x.ProviderId == providerId && x.Patient.MedicalStatus != "discharged" && x.Patient.MedicalStatus != "dead" && x.Active == true)
                .Select(x => x.Patient)
                .ProjectTo<ViewPatient>(config)
                .ToList();
        }


        /// <summary>
        /// Returns a list of all patients assigned to the provider by medicalstatus
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<ViewPatient>> GetAssignedPatientsByRole(string status)
        {
            //Gets user
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            //var providerId = provider.Id;
            var providerId = provider.Id;

            if (status == "Dead")
            {
                status = "death";
            }
            var config = new MapperConfiguration(cfg =>
             cfg.CreateMap<Patient, ViewPatient>()
               .ForMember(dto => dto.Diagnosis, conf => conf.MapFrom(x => x.DiagnosisCode.Diagnosis))
               .ForMember(dto => dto.NumProvidersAssigned, conf => conf.MapFrom(x => x.PatientProviders.Where(c => c.Provider.Role != "Administrator").Count()))
               );

            return db.PatientProviders
                .Where(x => x.ProviderId == providerId && x.Patient.MedicalStatus == status.ToLower() && x.Active == true)
                .Select(x => x.Patient)
                .ProjectTo<ViewPatient>(config)
                .ToList();
        }

        /// <summary>
        /// Updates a patient and changes their medical status to discharge
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<HttpResponseMessage> DischargePatient(int patientId)
        {
            //Checks user and makes sure they are a physician and are assigned to the patient
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (!cred.isAssigned(patientId) && provider.Role != "Physician")
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be assigned to this patient and be a physician to complete this task"
                });
            }
            var patient = db.Patients.Where(x => x.PatientId == patientId).Single();
            patient.MedicalStatus = "discharged";
            patient.DischargeDate = DateTime.Now;
            db.Entry(patient).State = EntityState.Modified;
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// our disposing method which closes our connection to db context
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
