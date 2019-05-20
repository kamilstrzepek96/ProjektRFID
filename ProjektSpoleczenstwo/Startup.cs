using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjektSpoleczenstwo.Startup))]
namespace ProjektSpoleczenstwo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
