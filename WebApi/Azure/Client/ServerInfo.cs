using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// Static class that returns the api url used to switch between local host and azure api
    /// </summary>
    public static class ServerInfo
    {
        public static string ServerName()
        {
            //return "http://localhost:34992/";
            return "https://mardenfinalproject.azurewebsites.net/";
        }
    }
}
