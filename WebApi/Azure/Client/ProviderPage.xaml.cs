using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
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

namespace Client
{
    /// <summary>
    /// Represents the list of providers assigned to the patient
    /// </summary>
    public sealed partial class ProviderPage : Page
    {
        //our view model for our page
        private PatientScreenData screenData = new PatientScreenData();

        //Represents our connection to our azure api
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        //Our list of providers
        private List<ViewPatientProvider> providers;

        public ProviderPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assigns data passed from previous page to view model and populates the image list
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.screenData = (PatientScreenData)e.Parameter;
            this.DataContext = this.screenData;
            MobileServiceDotNet.CurrentUser = new MobileServiceUser(screenData.auth.userid);
            MobileServiceDotNet.CurrentUser.MobileServiceAuthenticationToken = screenData.auth.token;
            populateProviderList();
            populatePatientProvider();
        }

        /// <summary>
        /// Gets list of all possible providers
        /// </summary>
        private async void populateProviderList()
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                MasterList.ItemsSource = await MobileServiceDotNet.InvokeApiAsync<List<ViewProvider>>("provider", HttpMethod.Get, null);
            }
            catch
            {
                var message = "There was an error while trying to get list of providers";
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
        /// Gets list of all providers for the patient
        /// </summary>
        private async void populatePatientProvider()
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientId"] = this.screenData.Patient.PatientId.ToString() };
                this.providers = await MobileServiceDotNet.InvokeApiAsync<List<ViewPatientProvider>>("assignment", HttpMethod.Get, parameters);
                ProviderList.ItemsSource = providers;
            }
            catch
            {
                var message = "There was an error while trying to get provider list for patient";
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
        /// Remove an assigned provider from the patient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void deleteProvider(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            //Get id of the patient provider assignment
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientProviderId"] = id };
                await MobileServiceDotNet.InvokeApiAsync("assignment", HttpMethod.Delete, parameters);
                populatePatientProvider();
            }
            catch
            {
                var message = "There was an error while trying to delete a provider";
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
        /// adds a provider to the patient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void addProvider(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            //get id of the provider and create the PatientProvier object
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            var assignment = createAssignment(id);
            try
            {
                if (this.providers.Any(x => x.ProviderId == Convert.ToInt32(id)))
                {
                    throw new InvalidDataException();
                }
                var data = JToken.FromObject(assignment);
                await MobileServiceDotNet.InvokeApiAsync("assignment", data);
                populatePatientProvider();

            }
            catch (InvalidDataException)
            {
                var message = "The provider you chose was already assigned";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            catch
            {
                var message = "There was an error while trying to add a provider";
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
        /// Creates the PatientProvider object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private PatientProvider createAssignment(string id)
        {
            PatientProvider pp = new PatientProvider();
            pp.AssignedDate = DateTime.Now;
            pp.ProviderId = Convert.ToInt32(id);
            pp.PatientId = this.screenData.Patient.PatientId;
            pp.Active = true;
            return pp;
        }

        /// <summary>
        /// Go back to patient page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToPatient(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientPage), this.screenData);
        }
    }
}
