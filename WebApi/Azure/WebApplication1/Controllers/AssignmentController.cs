using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.AuthenticationHelpers;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.Models;
using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Azure.Controllers
{
    /// <summary>
    /// Controller that governs the assignment between a provider and a patient
    /// </summary>
    [Authorize]
    public class AssignmentController : ApiController
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
        /// Creates a a new assignment between a patient and a provider
        /// </summary>
        /// <param name="pp"></param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Create(PatientProvider pp)
        {
            //Get User information and determine if administrator, reject request if not
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if(!cred.isAssigned(pp.PatientId))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be an administrator to complete this task"
                });
            }
            db.PatientProviders.Add(pp);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Deletes an existing assignment of a provider to a patient
        /// </summary>
        /// <param name="patientProviderId"></param>
        /// <returns>HttpResponseMessage</returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(int patientProviderId)
        {
            //Get User information and determine if administrator, reject request if not
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (provider.Role != "Administrator")
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be an administrator to complete this task"
                });
            }
            var pp = db.PatientProviders.Where(x => x.ProviderPatientId == patientProviderId).Single();
            db.PatientProviders.Remove(pp);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Gets a list of the providers assigned to a given patient
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns>List of patient provider assignments</returns>
        [HttpGet]
        public List<ViewPatientProvider> Get(int patientId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PatientProvider, ViewPatientProvider>()
                   .ForMember(dto => dto.Name, conf => conf.MapFrom(x => x.Provider.Name))
                   .ForMember(dto => dto.Role, conf => conf.MapFrom(x => x.Provider.Role));
            });

            var pp = db.PatientProviders.Where(x => x.PatientId == patientId && x.Active == true).ProjectTo<ViewPatientProvider>(config);
            return pp.ToList();
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
