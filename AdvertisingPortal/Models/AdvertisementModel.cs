using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdvertisingPortal.Models {
    [Table("Advertisements")]
    public class AdvertisementModel {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public bool ToNegotiate { get; set; }
        [Required]
        public DateTime AddTime { get; set; }
        public DateTime? CloseTime { get; set; }
        [Required]
        public bool Active { get; set; }

        public virtual UserModel User { get; set; }
        public virtual ICollection<FileModel> Files { get; set; }
        public virtual CategoryModel Category { get; set; }
        public virtual ICollection<FavouriteModel> Favourites { get; set; }
    }

    public class AdvertisementCreate {
        public AdvertisementModel advertisement { get; set; }
        //[Required]
        //public CategoryModel category { get; set; }
        [Required]
        public HttpPostedFile file { get; set; }
    }
}