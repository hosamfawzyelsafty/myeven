using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(myevenService.Startup))]

namespace myevenService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}