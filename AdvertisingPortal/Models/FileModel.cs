using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdvertisingPortal.Models {
    [Table("Files")]
    public class FileModel {
        public int ID { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public DateTime AddTime { get; set; }

        public virtual AdvertisementModel Advertisement { get; set; }
    }
}