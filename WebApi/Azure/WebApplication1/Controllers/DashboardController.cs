using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.DataObjects;
using Azure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.DashboardObjects;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// Controller used to generate aggregates for web dashboard
    /// </summary>
    public class DashboardController : ApiController
    {
        /// <summary>
        /// our dbcontext that represents our Azure SQL Database
        /// </summary>
        private DataContext db = new DataContext();

        /// <summary>
        /// Gets Patient related information from the dashboard
        /// </summary>
        /// <returns>Collection of Patient Dashboard Object</returns>
        private IEnumerable<PatientDashboard> PatientInfo()
        {
            var config = new MapperConfiguration(cfg =>
              cfg.CreateMap<Patient, PatientDashboard>()
                .ForMember(dto => dto.procedures, conf => conf.MapFrom(x => x.PatientProcedures.Select(c => c.ProcedureCode.Procedure)))
                .ForMember(dto => dto.diagnosis, conf => conf.MapFrom(x => x.DiagnosisCode.Diagnosis))
                .ForMember(dto => dto.chatCount, conf => conf.MapFrom(x => x.PatientChatLogs.Count()))
                .ForMember(dto => dto.imageCount, conf => conf.MapFrom(x => x.PatientImagings.Count()))
                .ForMember(dto => dto.providerCount, conf => conf.MapFrom(x => x.PatientProviders.Where(c => c.Provider.Role != "Administrator").Count()))
             );

            return db.Patients.ProjectTo<PatientDashboard>(config);
        }

        /// <summary>
        /// Gets provider related information from the dashboard
        /// </summary>
        /// <returns>Collection of Provider Dashboard Object</returns>
        private IEnumerable<ProviderDashboard> ProviderInfo()
        {
            var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Provider, ProviderDashboard>()
                        .ForMember(dto => dto.procedures, conf => conf.MapFrom(x => x.PatientProcedures.Select(c=>c.ProcedureCode.Procedure)))
                        .ForMember(dto => dto.patients, conf => conf.MapFrom(x => x.PatientProviders.Select(c => c.Patient)));


                    cfg.CreateMap<Patient, PatientSimple>()
                        .ForMember(dto => dto.diagnosis, conf => conf.MapFrom(x => x.DiagnosisCode.Diagnosis));
                }
            );

            return db.Providers.ProjectTo<ProviderDashboard>(config);
        }

        /// <summary>
        /// Gets a list of diagnoses
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Item> GetDiagnoses()
        {
            return db.DiagnosisCodes.Select(x => new Item
            {
                id = x.DiagnosisCodeId,
                name = x.Diagnosis
            });
        }

        /// <summary>
        /// Gets a list of procedures
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Item> GetProcedures()
        {
            return db.ProcedureCodes.Select(x => new Item
            {
                id = x.ProcedureCodeId,
                name = x.Procedure
            });
        }


        /// <summary>
        /// A route exposed to the web that will use other methods on the controller in order to gather metrics and send to the client
        /// </summary>
        /// <returns>DataInfo Object</returns>
        [HttpGet]
        public DataInfo GetMetrics()
        {
            return new DataInfo
            {
                diagnoses = GetDiagnoses(),
                patients = PatientInfo(),
                providers = ProviderInfo(),
                procedures = GetProcedures()
            };
        }

    }
}
