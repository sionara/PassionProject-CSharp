using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(myPassionProject.Startup))]
namespace myPassionProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
