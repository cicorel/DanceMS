using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DanceMSService.Startup))]

namespace DanceMSService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}