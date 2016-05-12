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
    public class ProcedureCodeController : ApiController
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
        /// Get a list of possible procedures that can done
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<ProcedureCode> Get()
        {
            return db.ProcedureCodes.ToList();
        }

        /// <summary>
        /// Takes a patient id and gets all assigned procedures ot that patient
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetByPatient(int patientId)
        {
            //Must be assigned to the patient in order to get a list of their procedures
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (!cred.isAssigned(patientId))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be assigned to this physician to complete this task"
                });
            }

            var config = new MapperConfiguration(cfg =>
             cfg.CreateMap<PatientProcedure, ViewPatientProcedure>()
               .ForMember(dto => dto.Procedure, conf => conf.MapFrom(x => x.ProcedureCode.Procedure))
               .ForMember(dto => dto.CompletedTime, conf => conf.MapFrom(x=>x.CompletedTime))
               .ForMember(dto => dto.CompletedBy, conf => conf.MapFrom(x=>x.Provider.Name))
               .ForMember(dto => dto.procedureRole, conf => conf.MapFrom(x => x.ProcedureCode.Role))
            );

            var data = db.PatientProcedures.Where(x => x.PatientId == patientId).ProjectTo<ViewPatientProcedure>(config).ToList();
            data.ForEach(x => x.ShowRules.procedureRole = x.procedureRole);
            data.ForEach(x => x.ShowRules.Completed = x.Completed);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        /// <summary>
        /// Get a list of procedures by the procedure type which is an indication of who can complete the procedure
        /// </summary>
        /// <param name="role"></param>
        /// <returns>Collection of Procedures</returns>
        [HttpGet]
        public List<ViewSupportProcedure> GetProcedureByRole(string role)
        {
            var config = new MapperConfiguration(cfg =>
             cfg.CreateMap<PatientProcedure, ViewSupportProcedure>()
               .ForMember(dto => dto.ProcedureName, conf => conf.MapFrom(x => x.ProcedureCode.Procedure))
               .ForMember(dto => dto.PatientName, conf => conf.MapFrom(x => x.Patient.Name))
               .ForMember(dto => dto.PatientId, conf => conf.MapFrom(x => x.Patient.PatientId))
            );

            var data = db.PatientProcedures.Where(x => x.ProcedureCode.Role == role && x.Completed == false)
                .ProjectTo<ViewSupportProcedure>(config).ToList();
            return data;
        }

        /// <summary>
        /// Assigns a procedure to a patient
        /// </summary>
        /// <param name="procedure"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Create(PatientProcedure procedure)
        {
            //Must be assigned to the patient and must be a physician or surgeon
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if ((provider.Role != "Physician" && provider.Role != "Surgeon") && !cred.isAssigned(procedure.PatientId))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be this Patients Physician or Surgeon to complete this task"
                });
            }
            db.PatientProcedures.Add(procedure);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Completes an assigned procedure.
        /// </summary>
        /// <param name="patientProcedureId"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<HttpResponseMessage> Complete(int patientProcedureId)
        {
            // Must be assigned to the patient or be a nurse in which case they can complete procedures without being assigned
            var patientProcedure = db.PatientProcedures.Where(x => x.PatientProcedureId == patientProcedureId).Single();
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (!cred.isAssigned(patientProcedure.PatientId) && (provider.Role != "Nurse"))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be this Patients Physician or Surgeon to complete this task"
                });
            }
            patientProcedure.Completed = true;
            patientProcedure.ProviderId = provider.Id;
            patientProcedure.CompletedTime = DateTime.Now;
            db.Entry(patientProcedure).State = EntityState.Modified;
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// disposes our context
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
