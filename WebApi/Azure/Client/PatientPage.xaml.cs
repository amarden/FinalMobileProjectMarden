using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
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
    /// Patient's home page, contains all the links to other Patient data and all the meta data on the Patient
    /// </summary>
    public sealed partial class PatientPage : Page
    {
        //Represents our connection to our azure api
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        //our view model for our page
        private PatientScreenData screenData = new PatientScreenData();

        public PatientPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assigns data passed from previous page to view model and populates the procedure list
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            this.screenData = (PatientScreenData)e.Parameter;
            MobileServiceDotNet.CurrentUser = new MobileServiceUser(screenData.auth.userid);
            MobileServiceDotNet.CurrentUser.MobileServiceAuthenticationToken = screenData.auth.token;
            getPatient(screenData.Patient.PatientId);
        }

        /// <summary>
        /// business logic that determines whether to show hide the discharge button and the discharge data
        /// </summary>
        private void hideShowDischargeOrDead()
        {
            var patient = this.screenData.Patient;
            var user = this.screenData.User;
            if ((patient.MedicalStatus == "dead" || patient.MedicalStatus == "discharged"))
            {
                DischargeBtn.Visibility = Visibility.Collapsed;
                ProviderNum.Visibility = Visibility.Collapsed;
                ChatNum.Visibility = Visibility.Collapsed;
                ImageNum.Visibility = Visibility.Collapsed;
                ProcedureNum.Visibility = Visibility.Collapsed;
                ProcedureNumText.Visibility = Visibility.Collapsed;
                ImageNumText.Visibility = Visibility.Collapsed;
                ChatNumText.Visibility = Visibility.Collapsed;
                ProviderNumText.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// business logic that determines whether to show hide the discharge button and the discharge data
        /// </summary>
        private void hideShowDischargeBtn()
        {
            var patient = this.screenData.Patient;
            var user = this.screenData.User;
            if ((patient.MedicalStatus == "stable" || patient.MedicalStatus == "critical") && user.Role == "Physician")
            {
                DischargeBtn.Visibility = Visibility.Visible;
            }
            else
            {
                DischargeBtn.Visibility = Visibility.Collapsed;
            }
            if(patient.MedicalStatus == "discharge")
            {
                DischargeDateText.Text = "Discharge Data: " + this.screenData.Patient.DischargeDate.Value.ToString();
            }
        }

        /// <summary>
        /// Gets all the patient level information and attaches it to the view model.
        /// </summary>
        /// <param name="patientId"></param>
        private async void getPatient(int patientId)
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientId"]=patientId.ToString() };
                var resultJson = await MobileServiceDotNet.InvokeApiAsync<PatientDetail>("Patient", HttpMethod.Get, parameters);
                if (resultJson != null)
                {
                    this.screenData.Patient = resultJson;
                    BiometricList.ItemsSource = screenData.Patient.Biometrics;
                    this.DataContext = screenData;
                    hideShowDischargeBtn();
                }
            }
            catch(Exception e)
            {
                var message = "There was an error retrieving this patient "+ e.Message;
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
        /// Navigates to the provider page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToProviders(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ProviderPage), this.screenData);
        }

        /// <summary>
        /// Navigates to the procedure page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToProcedure(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ProcedurePage), this.screenData);
        }

        /// <summary>
        /// Navigates to the patients image page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToImage(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ImagingPage), this.screenData);
        }

        /// <summary>
        /// Navigates to the patients chat page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToChat(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChatPage), this.screenData);
        }

        /// <summary>
        /// Discharges the patient, can only be done by physician
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void discharge(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                var parameters = new Dictionary<string, string> { ["patientId"] = this.screenData.Patient.PatientId.ToString() };
                await MobileServiceDotNet.InvokeApiAsync("Patient", HttpMethod.Put, parameters);
                getPatient(this.screenData.Patient.PatientId);
                hideShowDischargeBtn();
                hideShowDischargeOrDead();
            }
            catch
            {
                var message = "There was an error while trying to discharge this patient";
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
        /// Navigates back to list of patients assigned to user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backToPatients(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HomePage), this.screenData);
        }
    }
}
