using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("First name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last name")]
        public string LastName { get; set; }
        [DisplayName("Phone number")]
        public int PhoneNumber { get; set; }
        [DisplayName("City")]
        public string City { get; set; }

        public virtual ICollection<AdvertisementModel> Advertisements { get; set; }
        public virtual ICollection<FavouriteModel> Favourites { get; set; }
    }

    public class UserRoleViewModel {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
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