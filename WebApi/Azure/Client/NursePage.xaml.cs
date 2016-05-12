using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Web.ClientObjects;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NursePage : Page
    {
        //Represents our connection to our azure api
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        //our view model for our page
        private User user = new User();
        private AuthInfo auth = new AuthInfo();

        public NursePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assigns data passed from previous page to view model and populates the procedure list
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var psd = (PatientScreenData)e.Parameter;
            this.user = psd.User;
            this.auth = psd.auth;
            MobileServiceDotNet.CurrentUser = new MobileServiceUser(psd.auth.userid);
            MobileServiceDotNet.CurrentUser.MobileServiceAuthenticationToken = psd.auth.token;
            this.DataContext = this.user;
            getSupportProcedures();
        }

        /// <summary>
        /// This gets the support procedures that populates the view list
        /// </summary>
        private async void getSupportProcedures()
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                var parameters = new Dictionary<string, string> { ["role"] = "Support" };
                var procedures = await MobileServiceDotNet.InvokeApiAsync<List<ViewSupportProcedure>>("procedurecode", HttpMethod.Get, parameters);
                ProcedureList.ItemsSource = procedures;
            }
            catch
            {
                var message = "There was an error retrieving this patient";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            finally
            {
                MyProgressBar.IsIndeterminate = false;
            }
        }

        /// <summary>
        /// This complete's the procedure and requeries the support type procedures 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void completeProcedure(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientProcedureId"] = id.ToString() };
                await MobileServiceDotNet.InvokeApiAsync("procedurecode", HttpMethod.Put, parameters);
                getSupportProcedures();
            }
            catch
            {
                var message = "There was an error while trying to complete a procedure";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            finally
            {
                MyProgressBar.IsIndeterminate = false;
            }
        }

        /// <summary>
        /// Navigates to the patients chat page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToChat(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.Last();
            var id = idElement.Text;
            PatientScreenData psd = new PatientScreenData();
            psd.User = this.user;
            psd.auth = this.auth;
            psd.Patient = new PatientDetail();
            psd.Patient.PatientId = Convert.ToInt32(id);
            this.Frame.Navigate(typeof(ChatPage), psd);
        }

        /// <summary>
        /// log out and go back to landing page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void logout(object sender, TappedRoutedEventArgs e)
        {
            await MobileServiceDotNet.LogoutAsync();
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
