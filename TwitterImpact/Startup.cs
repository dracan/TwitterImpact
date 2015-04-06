using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TwitterImpact.Startup))]
namespace TwitterImpact
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
