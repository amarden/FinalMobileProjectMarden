using Azure.DataObjects;
using System;
using System.Collections.Generic;

namespace Azure.ClientObjects
{
    /// <summary>
    /// Class that represents a chat message that is used by the client application
    /// </summary>
    public class ViewChatLog
    {
        public string ProviderName { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
    }
}