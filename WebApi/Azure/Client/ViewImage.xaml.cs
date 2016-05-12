using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Client
{
    /// <summary>
    /// Page that is used to view a patient image
    /// </summary>
    public sealed partial class ViewImage : Page
    {
        //Represents our connection to our azure api
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        //our view model for our page
        ImageNavScreenData screenData;

        public ViewImage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assigns data passed from previous page to view model and grabs the image data
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.screenData = (ImageNavScreenData)e.Parameter;
            this.DataContext = this.screenData.screenData;
            MobileServiceDotNet.CurrentUser = new MobileServiceUser(screenData.auth.userid);
            MobileServiceDotNet.CurrentUser.MobileServiceAuthenticationToken = screenData.auth.token;
            showImage(this.screenData.BlobId);
        }

        /// <summary>
        /// query to get the image data
        /// </summary>
        /// <param name="id"></param>
        private async void showImage(string id)
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["blobId"] = id, ["patientId"] = this.screenData.screenData.Patient.PatientId.ToString() };
                byte[] imageData = await MobileServiceDotNet.InvokeApiAsync<byte[]>("patientImaging", HttpMethod.Get, parameters);
                //convert byte data in filestream
                using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
                {
                    using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes(imageData);
                        await writer.StoreAsync();
                    }

                    var image = new BitmapImage();
                    await image.SetSourceAsync(ms);
                    Scan.Source = image;
                }
            }
            catch
            {
                var message = "There was an error retrieving the Image";
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
        /// Go back to patient image pages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToImages(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ImagingPage), this.screenData.screenData);
        }
    }
}
