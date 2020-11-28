using AdvertisingPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AdvertisingPortal.DAL {
    public class AdvertisementPortalContext : DbContext {

        public AdvertisementPortalContext() : base("name=DefaultConnection") {
            Database.SetInitializer(new DatabaseInitializer<AdvertisementPortalContext>());
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<AdvertisementModel> Advertisements { get; set; }
        public DbSet<FileModel> Files { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<FavouriteModel> Favourites { get; set; }
    }
}