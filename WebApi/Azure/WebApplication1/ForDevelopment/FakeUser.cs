using Azure.ClientObjects;
using Azure.DataObjects;
using Azure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Temporary
{
    /// <summary>
    /// Used to select a user in provider to help test functionality
    /// </summary>
    public static class FakeUser
    {
        public static User getUser()
        {
            using (var db = new DataContext())
            {
                var prov = db.Providers.Select(x=> new User
                {
                    Id = x.ProviderId,
                    Name = x.Name,
                    Role = x.Role
                }).ToList();
                return prov[5];
            }
        }
    }
}
