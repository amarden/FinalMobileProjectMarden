using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.ClientObjects
{
    /// <summary>
    /// Class used to represent information being submitted by a POST call to create an image
    /// </summary>
    public class SubmitImage
    {
        public int PatientId { get; set; }
        public string ImageType { get; set; }
        public byte[] ImageStream { get; set; }
    }
}
