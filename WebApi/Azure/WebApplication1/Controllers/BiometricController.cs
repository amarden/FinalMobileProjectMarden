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
    public class BiometricController : ApiController
    {
        private DataContext db = new DataContext();

        [HttpGet]
        public List<Biometric> GetMeasurements(int patientId)
        {
            return db.Biometrics.Where(x => x.PatientId == patientId).OrderByDescending(x => x.MeasurementDate).ToList();
        }

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
