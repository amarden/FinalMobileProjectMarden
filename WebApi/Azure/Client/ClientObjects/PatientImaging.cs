using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client.ClientObjects
{
    /// <summary>
    /// Represents a Patient's image
    /// </summary>
    public class PatientImaging
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientImagingId{ get; set; }
        public int PatientId { get; set; }
        public string ImageType { get; set; }
        public string ImageBlobId { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
