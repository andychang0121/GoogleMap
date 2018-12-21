using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoogleMap.Web.Startup))]
namespace GoogleMap.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
