using Client.ClientObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Web.ClientObjects;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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

namespace Client
{
    /// <summary>
    /// Represents the page where a list of images associated with patient can be seen
    /// </summary>
    public sealed partial class ImagingPage : Page
    {
        //our view model for our page
        private PatientScreenData screenData = new PatientScreenData();

        //Represents our connection to our azure api
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient(ServerInfo.ServerName());

        //class level variable that keeps track of imageType that the user wants to create
        private string imageType { get; set; }

        public ImagingPage()
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
            populateImages();
        }

        /// <summary>
        /// Gets list of images associated with the patient
        /// </summary>
        private async void populateImages()
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string> { ["patientId"] = this.screenData.Patient.PatientId.ToString() };
                List<PatientImaging> patientImages = await MobileServiceDotNet.InvokeApiAsync<List<PatientImaging>>("patientimaging", HttpMethod.Get, parameters);
                ImageList.ItemsSource = patientImages;
            }
            catch
            {
                var message = "There was an error retrieving your images";
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
        /// Adds an image to the api
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void addImage(object sender, RoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            try
            {
                if (this.imageType == "" || this.imageType == null)
                {
                    throw new Exception();
                }
                FileOpenPicker fp = new FileOpenPicker(); // Adding filters for the file type to access.         
                fp.FileTypeFilter.Add(".jpeg");
                fp.FileTypeFilter.Add(".png");
                fp.FileTypeFilter.Add(".jpg");
                //Opens up file directory to choose image file
                StorageFile sf = await fp.PickSingleFileAsync();

                //Creates byte array data from image file
                byte[] fileBytes = null;
                using (IRandomAccessStreamWithContentType stream = await sf.OpenReadAsync())
                {
                    fileBytes = new byte[stream.Size];
                    using (DataReader reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        reader.ReadBytes(fileBytes);
                    }
                }
                BitmapImage tempBitMap = new BitmapImage(new Uri(sf.Path));
                PatientImage.Source = tempBitMap;
                PatientImage.Visibility = Visibility.Visible;
                Stream fileStream = new MemoryStream(fileBytes);
                submitImage(fileBytes);
            }
            catch
            {
                var message = "Please choose image type before uploading an image";
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
        /// Submits the image data to create the image
        /// </summary>
        /// <param name="imageStream"></param>
        private async void submitImage(byte[] imageStream)
        {
            MyProgressBar.IsIndeterminate = true;
            var newImage = new SubmitImage();
            try
            {
                newImage.ImageStream = imageStream;
                newImage.PatientId = this.screenData.Patient.PatientId;
                newImage.ImageType = this.imageType;
                var data = JToken.FromObject(newImage);
                await MobileServiceDotNet.InvokeApiAsync("patientimaging", data);
                this.imageType = "";
                populateImages();
            }
            catch
            {
                var message = "There was an error while trying to upload this image";
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
        /// Go back to patient page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navToPatient(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PatientPage), this.screenData);
        }

        /// <summary>
        /// Called when someone checks one of the image types, assigns it to the imageType
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var obj = (RadioButton)sender;
            this.imageType = obj.Content.ToString();
        }

        /// <summary>
        /// Gets blobId that represents image location on blob and navigates to new page to show image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewImage(object sender, TappedRoutedEventArgs e)
        {
            MyProgressBar.IsIndeterminate = true;
            var button = (Button)sender;
            var grid = (Grid)button.Parent;
            var idElement = (TextBlock)grid.Children.First();
            var id = idElement.Text;
            var data = new ImageNavScreenData() { BlobId = id, screenData = this.screenData };
            data.auth = this.screenData.auth;
            this.Frame.Navigate(typeof(ViewImage), data);
        }
    }
}
