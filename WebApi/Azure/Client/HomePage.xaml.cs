using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Web.ClientObjects;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Client
{
    /// <summary>
    /// The landing page for administrators, physicians, and surgeons
    /// </summary>
    public sealed partial class HomePage : Page
    {
        //Represents our connection to our azure api
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        //declare new user class
        private User user = new User();
        private PatientScreenData psd = new PatientScreenData();

        public HomePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assigns user that comes from MainPage to our view model
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PatientScreenData screenData = (PatientScreenData)e.Parameter;
            this.psd = screenData;
            this.user = screenData.User;
            MobileServiceDotNet.CurrentUser = new MobileServiceUser(screenData.auth.userid);
            MobileServiceDotNet.CurrentUser.MobileServiceAuthenticationToken = screenData.auth.token;
            this.DataContext = this.user;
            getAssignedPatients();
        }

        /// <summary>
        /// Queries our api to get all provider's patients
        /// </summary>
        private async void getAssignedPatients()
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                var resultJson = await MobileServiceDotNet.InvokeApiAsync<List<Patient>>("Patient", HttpMethod.Get, null);
                if (resultJson != null)
                {
                    if (resultJson.Count == 0)
                    {
                        NoAssignedText.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        NoAssignedText.Visibility = Visibility.Collapsed;
                    }
                    PatientList.ItemsSource = resultJson;
                }
            }
            catch (Exception exc)
            {
                var message = "There was an error retrieving your patients";
                var dialog = new MessageDialog(message + "\n " + exc.Message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            finally
            {
                MyProgressBar.IsIndeterminate = false;
            }
        }

        /// <summary>
        /// Navigates to PatientPage passing patient level data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToPatientPage(object sender, TappedRoutedEventArgs e)
        {
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            PatientScreenData screenData = new PatientScreenData();
            screenData.Patient = new PatientDetail();
            screenData.auth = this.psd.auth;
            screenData.Patient.PatientId = Convert.ToInt32(id);
            screenData.User = this.user;
            this.Frame.Navigate(typeof(PatientPage), screenData);
        }

        /// <summary>
        /// Queries our api to get all provider's patients by a certain status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void searchByStatus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                //Grabs status
                var obj = (RadioButton)sender;
                string status = obj.Content.ToString();

                Dictionary<string, string> parameters = new Dictionary<string, string> { ["status"] = status };
                if(status == "Default")
                {
                    parameters = null;
                }
                var resultJson = await MobileServiceDotNet.InvokeApiAsync<List<Patient>>("Patient", HttpMethod.Get, parameters);
                if (resultJson != null)
                {
                    PatientList.ItemsSource = resultJson;
                }
            }
            catch
            {
                var message = "There was an error retrieving patients";
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
        /// This navigates to the search page for administrators
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToSearch(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientSearch), this.psd);
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
