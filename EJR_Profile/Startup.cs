using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EJR_Profile.Startup))]
namespace EJR_Profile
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
