using AdvertisingPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AdvertisingPortal.DAL {
    public class DatabaseInitializer<T> : CreateDatabaseIfNotExists<AdvertisementPortalContext> {
        protected override void Seed(AdvertisementPortalContext context) {
            UserModel user = new Models.UserModel() {
                FirstName = "Jan",
                LastName = "Kowalski",
                PhoneNumber = 123456789,
                Email = "example@gmail.com",
                City = "Łódź"
            };

            context.Users.Add(user);
        }
    }
}