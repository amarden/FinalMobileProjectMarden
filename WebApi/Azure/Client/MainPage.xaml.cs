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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Client
{
    /// <summary>
    /// Home landing page of app where someone can register or user can login
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Represents our connection to our azure api
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());
        AuthInfo auth = new AuthInfo();

        /// <summary>
        /// class used to post for user registration to carry what role the user wants to be
        /// </summary>
        public class ProviderType
        {
            public string role { get; set; }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Logs the user in using twitter, on successful login will navigate to the page associated with the user's role
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void login(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                var stuff = await MobileServiceDotNet.LoginAsync(MobileServiceAuthenticationProvider.Google);
                this.auth.userid = stuff.UserId;
                this.auth.token = stuff.MobileServiceAuthenticationToken;
            }
            catch
            {
                var message = "There was a problem during login, you must login to use app";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            try
            {
                PatientScreenData screenData = new PatientScreenData();
                var resultJson = await MobileServiceDotNet.InvokeApiAsync<User>("login", HttpMethod.Get, null);
                screenData.User = resultJson;
                screenData.auth = this.auth;
                if (resultJson != null)
                {
                    if (resultJson.Role == "Nurse")
                    {
                        this.Frame.Navigate(typeof(NursePage), screenData);
                    }
                    else if(resultJson.Role == "SuperUser")
                    {
                        this.Frame.Navigate(typeof(SuperUserPage), screenData);
                    }
                    else
                    {
                        this.Frame.Navigate(typeof(HomePage), screenData);
                    }
                }
            }
            catch (MobileServiceInvalidOperationException)
            {
                var message = "You have not registered or logged in. Please do so";
                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
            catch
            {
                var message = "There was an error retrieving your profile";
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
        /// Registers the user 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void register(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var btn = (Button)sender;
            //get role and create providertype object
            var role = btn.Content;
            ProviderType pt = new ProviderType();
            pt.role = role.ToString();
            try
            {
                await MobileServiceDotNet.LoginAsync(MobileServiceAuthenticationProvider.Google);
                var data = JToken.FromObject(pt);
                await MobileServiceDotNet.InvokeApiAsync("registration", data);
            }
            catch
            {
                var message = "There was a problem during registration";
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
