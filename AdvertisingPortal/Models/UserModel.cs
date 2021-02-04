using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdvertisingPortal.Models {
    [Table("Users")]
    public class UserModel {
        [Key]
        public string ID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }
        public string City { get; set; }

        public virtual ICollection<AdvertisementModel> Advertisements { get; set; }
        public virtual ICollection<FavouriteModel> Favourites { get; set; }
    }

    public class RegisterUserModel {
        public RegisterViewModel registerInfo { get; set; }
        public UserModel userInfo { get; set; }

        public RegisterUserModel() { }

        public RegisterUserModel(RegisterViewModel registerInfo, UserModel userInfo) {
            this.registerInfo = registerInfo;
            this.userInfo = userInfo;
        }
    }

    public class CompleteUserModel {
        public IdentityUser user { get; set; }
        public UserModel userInfo { get; set; }

        public CompleteUserModel() { }

        public CompleteUserModel(IdentityUser registerInfo, UserModel userInfo) {
            this.user = registerInfo;
            this.userInfo = userInfo;
        }
    }
}