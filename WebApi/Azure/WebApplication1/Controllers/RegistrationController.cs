using Azure.AuthenticationHelpers;
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

namespace WebApplication1.Controllers
{
    /// <summary>
    /// Controller used to register a new provider
    /// </summary>
    [Authorize]
    public class RegistrationController : ApiController
    {
        /// <summary>
        /// our dbcontext that represents our Azure SQL Database
        /// </summary>
        private DataContext db = new DataContext();

        //Config Settings that we pass to our Credentials Class
        public MobileAppSettingsDictionary ConfigSettings => Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

        /// <summary>
        /// Creates a new user in our provider table
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> CreateUser(ProviderType type)
        {
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var userInfo = await cred.GetAuthInfo();
            var potentialId = userInfo.UserId;
            var exist = db.Providers.Where(x => x.TwitterUserId == potentialId);
            if (exist.Count() > 0)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Message = "User has already been Registered",
                });
            }
            Provider p = new Provider();
            p.Name = userInfo.Name;
            p.TwitterUserId = potentialId;
            p.Role = type.role;
            db.Providers.Add(p);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        /// <summary>
        /// disposes our context
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Used by registration route to represent the role that the user who is registering will be
    /// </summary>
    public class ProviderType
    {
        public string role { get; set; }
    }
}
