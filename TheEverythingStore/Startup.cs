using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheEverythingStore.Startup))]
namespace TheEverythingStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
