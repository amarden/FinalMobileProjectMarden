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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Client
{
    /// <summary>
    /// Procedure list page for a patient
    /// </summary>
    public sealed partial class ProcedurePage : Page
    {
        //our view model for our page
        private PatientScreenData screenData = new PatientScreenData();
        
        //Represents our connection to our azure api
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        public ProcedurePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assigns data passed from previous page to view model and populates the procedure list
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.screenData = (PatientScreenData)e.Parameter;
            this.DataContext = this.screenData;
            MobileServiceDotNet.CurrentUser = new MobileServiceUser(screenData.auth.userid);
            MobileServiceDotNet.CurrentUser.MobileServiceAuthenticationToken = screenData.auth.token;
            populateProcedureList();
            populatePatientProcedures();
        }

        /// <summary>
        /// populates list of procedures that are possible to assign to a patient
        /// </summary>
        private async void populateProcedureList()
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                MasterList.ItemsSource = await MobileServiceDotNet.InvokeApiAsync<List<ProcedureCode>>("procedurecode", HttpMethod.Get, null);
            }
            catch
            {
                var message = "There was an error while trying to get procedures";
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
        /// Populate procedures assigned to this patient
        /// </summary>
        private async void populatePatientProcedures()
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientId"] = this.screenData.Patient.PatientId.ToString() };
                List<ViewPatientProcedure> patientProcedures = await MobileServiceDotNet.InvokeApiAsync<List<ViewPatientProcedure>>("procedurecode", HttpMethod.Get, parameters);
                patientProcedures.ForEach(x => x.ShowRules.userRole = this.screenData.User.Role);
                ProcedureList.ItemsSource = patientProcedures;
            }
            catch
            {
                var message = "There was an error while trying to get providers";
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
        /// adds a procedure to the patient 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void addProcedure(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            //get the id of the procedure
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            var assignment = createAssignment(id);
            try
            {
                var data = JToken.FromObject(assignment);
                await MobileServiceDotNet.InvokeApiAsync("procedurecode", data);
                populatePatientProcedures();
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
        /// takes the id of the procedure and then creates a PatientProcedure object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private PatientProcedure createAssignment(string id)
        {
            PatientProcedure pp = new PatientProcedure();
            pp.AssignedTime = DateTime.Now;
            pp.ProcedureCodeId = Convert.ToInt32(id);
            pp.PatientId = this.screenData.Patient.PatientId;
            pp.Completed = false;
            pp.CompletedTime = null;
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

        /// <summary>
        /// Completes a procedure assigned to the patient
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
                populatePatientProcedures();
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
    }
}
