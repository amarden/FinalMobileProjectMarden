using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.AuthenticationHelpers;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// Api that is used to query patient table through Azure search
    /// </summary>
    [Authorize]
    public class PatientSearchController : ApiController
    {
        /// <summary>
        /// our dbcontext that represents our Azure SQL Database
        /// </summary>
        private DataContext db = new DataContext();
        
        /// <summary>
        /// Settings used to pass through to our authentication to get information from twitter auth api
        /// </summary>
        public MobileAppSettingsDictionary ConfigSettings => Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

        /// <summary>
        /// Takes a patient name or partial name and uses Azure search to query list of patients that match the name
        /// </summary>
        /// <param name="patientName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> LookUpPatientByName(string patientName)
        {
            //Can only be accessed by Administrator
            Credentials cred = new Credentials(User, ConfigSettings, Request);
            var provider = await cred.GetUserInfo();
            if (provider.Role != "Administrator")
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Message = "Must be administrator to look up patients"
                });
            }

            //azure search credentials... understandably in a real app should not have this here
            string searchServiceName = "patient-ehr";
            string apiKey = "D55413CA4F3C8D5A2DC5BB51E535A072";

            //Generate the Azure search api object
            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            SearchIndexClient searchClient = serviceClient.Indexes.GetClient("temp");
            var results = SearchDocuments(searchClient, patientName);
            results.Results.ToList().ForEach(x => x.Document.match = x.Document.ProviderId == provider.Id);
            var patientResults = results.Results.Select(x => x.Document);
            return Request.CreateResponse(HttpStatusCode.OK, patientResults);
        }

        /// <summary>
        /// Method used to access the Execute the Azure search api
        /// </summary>
        /// <param name="indexClient"></param>
        /// <param name="searchText"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private DocumentSearchResult<PatientSearchModel> SearchDocuments(SearchIndexClient indexClient, string searchText, string filter = null)
        {
            // Execute search based on search text and optional filter
            var sp = new SearchParameters();

            if (!String.IsNullOrEmpty(filter))
            {
                sp.Filter = filter;
            }

            DocumentSearchResult<PatientSearchModel> response = indexClient.Documents.Search<PatientSearchModel>(searchText, sp);
            return response;
        }

        /// <summary>
        /// Class that represents the schema of what our azure search returns
        /// </summary>
        [SerializePropertyNamesAsCamelCase]
        public class PatientSearchModel
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string Gender { get; set; }
            public string MedicalStatus { get; set; }
            public string Diagnosis { get; set; }
            public int ProviderId { get; set; }
            public bool match { get; set; }
        }
    }
}
