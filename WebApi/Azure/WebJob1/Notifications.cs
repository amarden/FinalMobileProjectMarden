using Microsoft.Azure.NotificationHubs;

namespace WebJob1.NotificationClass
{
    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://finalprojectnotificationhubnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=seOolThqNKJQYREaI7TKDJLAFdFLzA5MmL5rASruiZQ=",
                                                                         "FinalProjectNotificationHub");
        }
    }
}