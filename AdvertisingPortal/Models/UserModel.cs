using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdvertisingPortal.Models {
    [Table("Users")]
    public class UserModel {
        public int ID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        public string City { get; set; }

        public virtual ICollection<AdvertisementModel> Advertisements { get; set; }
        public virtual ICollection<FavouriteModel> Favourites { get; set; }
    }
}