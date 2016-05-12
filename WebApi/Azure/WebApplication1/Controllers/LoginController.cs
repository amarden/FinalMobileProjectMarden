using Azure.AuthenticationHelpers;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.Models;
using Azure.Temporary;
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
    [Authorize]
    public class LoginController : ApiController
    {
        /// <summary>
        /// our dbcontext that represents our Azure SQL Database
        /// </summary>
        private DataContext db = new DataContext();

        //Config Settings that we pass to our Credentials Class
        public MobileAppSettingsDictionary ConfigSettings => Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

        /// <summary>
        /// Gets credentials and logs user in
        /// </summary>
        /// <returns>User object</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetUser()
        {
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var userInfo = await cred.GetAuthInfo();
            var potentialId = userInfo.UserId;
            var exist = db.Providers.Where(x => x.TwitterUserId == potentialId);
            // Makes sure the user is already registered
            if (!exist.Any())
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Message = "User has not registered",
                });
            }
            var user = new User
            {
                Id = exist.Single().ProviderId,
                Name = exist.Single().Name,
                Role = exist.Single().Role
            };
            //var user = FakeUser.getUser();
            return Request.CreateResponse(HttpStatusCode.OK, user);
        }
    }
}
