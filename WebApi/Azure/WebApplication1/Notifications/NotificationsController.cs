using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApplication1.Notifications
{
    public class NotificationsController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> Post(string pns)
        {
            var user = HttpContext.Current.User.Identity.Name;
            string[] userTag = new string[2];
            userTag[0] = "username: device";
            userTag[1] = "from: azure" ;
            string message = "programattic sending";
            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;
            switch (pns.ToLower())
            {
                case "wns":
                    // Windows 8.1 / Windows Phone 8.1
                    var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                                 message + "</text></binding></visual></toast>";
                    try
                    {
                        outcome = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast);
                    }
                    catch(Exception e)
                    {
                        string error = e.Message;
                    }
                    break;
            }

            if (outcome != null)
            {
                if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                    (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                {
                    ret = HttpStatusCode.OK;
                }
            }

            return Request.CreateResponse(ret);
        }
    }
}
