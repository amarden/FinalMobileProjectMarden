using Azure;
using Azure.AuthenticationHelpers;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class PatientImagingController : ApiController
    {
        private const string MY_CONTAINER = "patientimaging";

        /// <summary>
        /// our dbcontext that represents our Azure SQL Database
        /// </summary>
        private DataContext db = new DataContext();

        /// <summary>
        /// Settings used to pass through to our authentication to get information from twitter auth api
        /// </summary>
        public MobileAppSettingsDictionary ConfigSettings => Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

        /// <summary>
        /// Gets a list of images for a patient
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetByPatient(int patientId)
        {
            //Checks to make sure that the provider is assigned to the patient 
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (!cred.isAssigned(patientId))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be assigned to this patient to upload this image"
                });
            }

            var images = db.PatientImagings.Where(x => x.PatientId == patientId).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, images);
        }

        /// <summary>
        /// Gets a specific image based on guid
        /// </summary>
        /// <param name="blobId"></param>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetImage(string blobId, int patientId)
        {
            //Checks to make sure that the provider is assigned to the patient 
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (!cred.isAssigned(patientId))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be assigned to this patient to upload this image"
                });
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(WebApiConfig.StorageConnectionString);
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference(MY_CONTAINER);
            // Retrieve reference to a blob named the blob specified by the caller
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobId);
            MemoryStream stream = new MemoryStream();

            blockBlob.DownloadToStream(stream);
            return Request.CreateResponse(HttpStatusCode.OK, stream.ToArray());
        }

        /// <summary>
        /// Takes the SubmitImage object and creates an image reference object on AzureSql and puts image data on Azure blob
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> CreateImage(SubmitImage image)
        {
            //Checks to make sure that the provider is assigned to the patient 
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (!cred.isAssigned(image.PatientId))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be assigned to this patient to upload this image"
                });
            }

            //Creates the Image object to be saved in Azure atabase
            string guid = Guid.NewGuid().ToString();
            PatientImaging pi = new PatientImaging();
            pi.PatientId = image.PatientId;
            pi.UploadDate = DateTime.Now;
            pi.ImageBlobId = guid;
            pi.ImageType = image.ImageType;
            db.PatientImagings.Add(pi);

            //Passes the image data to Azure blob and creates reference based on guid
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(WebApiConfig.StorageConnectionString);
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference(MY_CONTAINER);
            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();
            // Set permissions on the blob container to prevent public access
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off });

            // Retrieve reference to a blob named the blob specified by the caller
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(guid);
            blockBlob.UploadFromStream(new MemoryStream(image.ImageStream));
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// our disposing method which closes our connection to db context
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
