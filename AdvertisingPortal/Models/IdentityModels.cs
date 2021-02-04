using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AdvertisingPortal.Models {
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false) {
            Database.SetInitializer(new IdentityDBInizializer<ApplicationDbContext>());
        }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }
    }

    public class IdentityDBInizializer<T> : CreateDatabaseIfNotExists<ApplicationDbContext> {
        protected override void Seed(ApplicationDbContext context) {
            context.Roles.Add(new IdentityRole { Name = "manager" });
            context.Roles.Add(new IdentityRole { Name = "admin" });
            context.Roles.Add(new IdentityRole { Name = "user" });
            context.SaveChanges();
        }
    }
}