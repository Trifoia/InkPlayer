using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Oqtane.Services;
using Trifoia.Module.InkPlayer.Services;

namespace Trifoia.Module.InkPlayer.Client.Services
{
    public class Startup : IClientStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<InkPlayerService, InkPlayerService>();
            services.AddMudServices();
        }
    }
}
