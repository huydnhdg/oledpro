using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BHDT_OledPro.Startup))]
namespace BHDT_OledPro
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
