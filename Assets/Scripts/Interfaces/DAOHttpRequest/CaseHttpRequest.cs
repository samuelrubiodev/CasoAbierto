using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
public class CaseHttpRequest : IDaoHttpRequest<JObject, StringContent>, IDaoHttpRequestExtended<JObject, StringContent>
{
    private readonly string urlBase = "http://" + Server.ACTIVE_CASE_HOST;
    private readonly HttpClient httpClient;

    public CaseHttpRequest()
    {
        httpClient = new()
        {
            BaseAddress = new Uri(urlBase)
        };
    }

    public async Task<JObject> PostAsync(string url, StringContent data = null)
    {
        try {
            var response = await httpClient.PostAsync(url, data);
            string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JObject.Parse(body);
        } catch (Exception) {
            throw new Exception("Error en la solicitud POST a " + url);
        }
    }

    public async Task<JObject> GetAsync(string url)
    {
        try {
            var response = await httpClient.GetAsync(url);
            string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JObject.Parse(body);
        } catch (Exception) {
            throw new Exception("Error en la solicitud GET a " + url);
        }
    }
}