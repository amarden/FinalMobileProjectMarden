using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Azure.Controllers
{
    /// <summary>
    /// Class used to query provider table
    /// </summary>
    [Authorize]
    public class ProviderController : ApiController
    {
        /// <summary>
        /// our dbcontext that represents our Azure SQL Database
        /// </summary>
        private DataContext db = new DataContext();

        //Automapper config to go from provider to ViewProvider
        private MapperConfiguration config = new MapperConfiguration(cfg =>
                cfg.CreateMap<Provider, ViewProvider>());

        /// <summary>
        /// Get All providers that are not nurses
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<ViewProvider> Get()
        {
            return db.Providers.Where(x=>x.Role != "Nurse" && x.Role != "SuperUser").ProjectTo<ViewProvider>(config).ToList();
        }

        /// <summary>
        /// Get a provider by a role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpGet]
        public List<ViewProvider> GetByRole(string role)
        {
            return db.Providers
                .Where(x=>x.Role == role)
                .ProjectTo<ViewProvider>(config).ToList();
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
