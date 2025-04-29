using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class GenerationIDHttpRequest : IDaoHttpRequestExtended<JObject, StringContent>
{
    private readonly string urlBase = "https://openrouter.ai/api/v1/";
    private readonly HttpClient httpClient;

    public GenerationIDHttpRequest()
    {
        httpClient = new()
        {
            BaseAddress = new Uri(urlBase)
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", ApiKey.API_KEY_OPEN_ROUTER);
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }
    public async Task<JObject> GetAsync(string relativeUrl)
    {
        var response = await httpClient.GetAsync(relativeUrl);
        string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JObject.Parse(body);
    }
}