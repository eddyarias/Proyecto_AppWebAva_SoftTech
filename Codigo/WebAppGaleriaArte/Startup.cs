using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebAppGaleriaArte.Startup))]


namespace WebAppGaleriaArte
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}