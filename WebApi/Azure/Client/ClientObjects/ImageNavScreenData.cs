using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ClientObjects
{
    /// <summary>
    /// Class that represents information passed to the ImageNavScreen
    /// </summary>
    public class ImageNavScreenData
    {
        public PatientScreenData screenData { get; set; }
        public string BlobId { get; set; }
        public AuthInfo auth { get; set; }
    }
}
