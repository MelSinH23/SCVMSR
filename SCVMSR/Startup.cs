using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SCVMSR.Startup))]
namespace SCVMSR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
