using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using System.Net;

namespace Trifoia.Module.InkPlayer.Services
{
    public class InkPlayerService : ResponseServiceBase, IService
    {
        public InkPlayerService(IHttpClientFactory http, SiteState siteState) : base(http, siteState) { }

        private string Apiurl => CreateApiUrl("InkPlayer");

        public async Task<(List<Models.InkPlayer>,HttpStatusCode)> GetInkPlayersAsync()
        {
            var url = $"{Apiurl}";
            (var data, var response) = await GetJsonWithResponseAsync<List<Models.InkPlayer>>(url);
            return (data, response.StatusCode);      
        }

        public async Task<(Models.InkPlayer,HttpStatusCode)> GetInkPlayerAsync(int id)
        {
            var url = $"{Apiurl}/{id}";
            (var data, var response) = await GetJsonWithResponseAsync<Models.InkPlayer>(url);
            return (data, response.StatusCode);        
        }

        public async Task<(Models.InkPlayer,HttpStatusCode)> AddInkPlayerAsync(Models.InkPlayer item)
        {
            var url = $"{Apiurl}";
            (var data, var response) = await PostJsonWithResponseAsync<Models.InkPlayer>(url,item);
            return (data, response.StatusCode);        
        }

        public async Task<(Models.InkPlayer,HttpStatusCode)> UpdateInkPlayerAsync(Models.InkPlayer item)
        {
            var url = $"{Apiurl}/{item.InkPlayerId}";
            (var data, var response) = await PutJsonWithResponseAsync<Models.InkPlayer>(url,item);
            return (data, response.StatusCode);        
        }

        public async Task<HttpStatusCode> DeleteInkPlayerAsync(int id)
        {
            var url = $"{Apiurl}/{id}";
            var response  = await DeleteWithResponseAsync(url);
            return response.StatusCode;
        }
    }
}
