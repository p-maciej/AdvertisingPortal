using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdvertisingPortal.Startup))]
namespace AdvertisingPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
