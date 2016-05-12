using Microsoft.Azure.WebJobs;
using System;
using WebJob1.EHRASsets;
using WebJob1.NotificationClass;

namespace WebJob1
{
    public class Functions
    {
        // This function will be triggered based on the schedule you have set for this WebJob
        // This function will enqueue a message on an Azure Queue called queue
        [NoAutomaticTrigger]
        public static void ManualTrigger()
        {
            Console.WriteLine("Change");
            var ehr = new EHR();
            ehr.PatientBiometricScan();
            Console.WriteLine("I am HerE");
        }

        [NoAutomaticTrigger]
        public async static void NotifyTrigger()
        {
            string message = "Message from the Webjob";
            // Windows 8.1 / Windows Phone 8.1
            var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                            message + "</text></binding></visual></toast>";
            try
            {
                await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in sending Notification message: " + e.Message);
            }
            Console.WriteLine("Triggered Notification");
        }
    }
}
