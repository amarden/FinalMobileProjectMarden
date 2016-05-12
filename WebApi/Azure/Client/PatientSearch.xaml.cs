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
    /// Page for administrators to search patients by name
    /// </summary>
    public sealed partial class PatientSearch : Page
    {
        //Represents our connection to our azure api
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        //our view model for our page
        private User user = new User();
        private PatientScreenData psd = new PatientScreenData();

        public PatientSearch()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assigns data passed from previous page to view model and populates the procedure list
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
        }

        /// <summary>
        /// queries our api that uses azure search. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void searchByName(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                //throw exception if no text was entered
                if (patientSearchText.Text == "")
                {
                    throw new NullReferenceException();
                }
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientName"] = patientSearchText.Text };
                var resultJson = await MobileServiceDotNet.InvokeApiAsync<List<PatientSearchModel>>("PatientSearch", HttpMethod.Get, parameters);
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
            catch (NullReferenceException)
            {
                var message = "Must enter text before searching";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
            }
            catch(Exception exc)
            {
                var message = "There was an error retrieving patients";
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
        /// Navigates back to home page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToHome(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HomePage), this.psd);
        }
    }
}
