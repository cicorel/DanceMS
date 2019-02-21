using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DanceMSBackend.Startup))]

namespace DanceMSBackend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}