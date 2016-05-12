using AutoMapper;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper.QueryableExtensions;
using Azure.AuthenticationHelpers;
using Microsoft.Azure.Mobile.Server;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace Azure.Controllers
{
    /// <summary>
    /// Controller that governs creations and view of chats during a patient
    /// </summary>
    [Authorize]
    public class ChatController : ApiController
    {
        /// <summary>
        /// Controller that governs the assignment between a provider and a patient
        /// </summary>
        private DataContext db = new DataContext();

        /// <summary>
        /// Settings used to pass through to our authentication to get information from twitter auth api
        /// </summary>
        public MobileAppSettingsDictionary ConfigSettings => Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

        /// <summary>
        /// Gets all messages for a given patient
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns>Chat messages for the patient passed</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Get(int patientId)
        {
            //Must be a provider assigned to the user to see the chat message
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (!cred.isAssigned(patientId) && provider.Role != "Nurse")
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be assigned to this patient to upload this image"
                });
            }

            var config = new MapperConfiguration(cfg =>
             cfg.CreateMap<PatientChatLog, ViewChatLog>()
             .ForMember(dto => dto.ProviderName, conf => conf.MapFrom(ol => ol.Provider.Name)));

            var chats =  db.PatientChatLogs
                .Where(x => x.PatientId == patientId)
                .OrderBy(x => x.Created)
                .ProjectTo<ViewChatLog>(config)
                .ToList();
            return Request.CreateResponse(HttpStatusCode.OK, chats);

        }

        /// <summary>
        /// Creates a new chat message based on the PatientChatLog object for a given patient
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Post(PatientChatLog message)
        {
            //Must be a provider assigned to the user to see the chat message or is nurse
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (!cred.isAssigned(message.PatientId) && provider.Role != "Nurse")
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be assigned to this patient to upload this image"
                });
            }

            db.PatientChatLogs.Add(message);
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
