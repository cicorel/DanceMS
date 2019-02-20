using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(dancemsService.Startup))]

namespace dancemsService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}