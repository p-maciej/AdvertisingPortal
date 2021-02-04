using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdvertisingPortal.Models {
    [Table("Categories")]
    public class CategoryModel {
        [Key]
        public int ID { get; set; }
        public int? Parent { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<AdvertisementModel> Advertisements { get; set; }
    }
}