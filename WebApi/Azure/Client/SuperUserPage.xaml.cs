using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Client
{
    /// <summary>
    /// Page that is the landing page for all super users
    /// </summary>
    public sealed partial class SuperUserPage : Page
    {
        //Represents our connection to our azure api
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        /// <summary>
        /// Class used to post the number of new patients to create
        /// </summary>
        public class PatientCreate
        {
            public int number { get; set; }
        }

        public SuperUserPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assigns auth info to service client
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PatientScreenData screenData = (PatientScreenData)e.Parameter;
            MobileServiceDotNet.CurrentUser = new MobileServiceUser(screenData.auth.userid);
            //MobileServiceDotNet.CurrentUser.UserId = screenData.auth.userid;
            MobileServiceDotNet.CurrentUser.MobileServiceAuthenticationToken = screenData.auth.token;
        }

        /// <summary>
        /// Creates new fake patients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void createPatients(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var numberText = (TextBox)FindName("numberOfPatients");
            var number = numberText.Text;
            if (await isNumber(number))
            {
                try
                {
                    PatientCreate pc = new PatientCreate();
                    pc.number = Convert.ToInt32(number);
                    var data = JToken.FromObject(pc);
                    await MobileServiceDotNet.InvokeApiAsync("patient", data);
                    //returns confirmation of the number of patients that were created
                    var message = number + " patients were created";
                    var dialog = new MessageDialog(message);
                    dialog.Commands.Add(new UICommand("OK"));
                    await dialog.ShowAsync();
                }
                catch
                {
                    var message = "There was a problem trying to create new patients";
                    var dialog = new MessageDialog(message);
                    dialog.Commands.Add(new UICommand("OK"));
                    await dialog.ShowAsync();
                }
                finally
                {
                    MyProgressBar.IsIndeterminate = false;
                }
            }
            
        }

        /// <summary>
        /// confirms that the user entered a number in the text box
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private async Task<bool> isNumber(string number)
        {
            int realNumber;
            if (Int32.TryParse(number, out realNumber))
            {
                return true;
            }
            else
            {
                var message = "You must enter a number";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
                return false;
            }
        }

        private async void logout(object sender, TappedRoutedEventArgs e)
        {
            await MobileServiceDotNet.LogoutAsync();
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
