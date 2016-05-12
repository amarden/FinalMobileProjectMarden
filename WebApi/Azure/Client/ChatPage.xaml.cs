using Azure.ClientObjects;
using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
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
    /// Represents the page where chat messages can be seen and a new chat can be submitted by the user
    /// </summary>
    public sealed partial class ChatPage : Page
    {
        //our view model for our page
        private PatientScreenData screenData = new PatientScreenData();
     
        //Represents our connection to our azure api
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        public ChatPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assigns data passed from previous page to view model and populates the chat window
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.screenData = (PatientScreenData)e.Parameter;
            this.DataContext = this.screenData;
            MobileServiceDotNet.CurrentUser = new MobileServiceUser(screenData.auth.userid);
            MobileServiceDotNet.CurrentUser.MobileServiceAuthenticationToken = screenData.auth.token;
            populateChat();
        }

        /// <summary>
        /// queries our api to get chat messages for the given patient
        /// </summary>
        private async void populateChat()
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientId"] = this.screenData.Patient.PatientId.ToString() };
                var chatLogs = await MobileServiceDotNet.InvokeApiAsync<List<ViewChatLog>>("chat", HttpMethod.Get, parameters);
                ChatLogList.ItemsSource = chatLogs;
            }
            catch
            {
                var message = "There was an error retrieving chat messages";
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
        /// Adds a message for our patient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void addMessage(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var messageElement = (TextBox)FindName("messageTextBox");
            var chatMessage = messageElement.Text;
            var chat = createMessage(chatMessage);
            messageElement.Text = "";
            try
            {
                var data = JToken.FromObject(chat);
                await MobileServiceDotNet.InvokeApiAsync("Chat", data);
                populateChat();
            }
            catch
            {
                var message = "There was an error while trying to add your message";
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
        /// This takes the string message and then returns a chat object used to post to the api
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private PatientChatLog createMessage(string message)
        {
            PatientChatLog chat = new PatientChatLog();
            chat.Created = DateTime.Now;
            chat.PatientId = this.screenData.Patient.PatientId;
            chat.ProviderId = this.screenData.User.Id;
            chat.Message = message;
            return chat;
        }

        /// <summary>
        /// Navigates to Patient Detail Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToPatient(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientPage), this.screenData);
        }

        /// <summary>
        /// Navigates to Nurse Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToNursePage(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NursePage), this.screenData);
        }
    }
}
