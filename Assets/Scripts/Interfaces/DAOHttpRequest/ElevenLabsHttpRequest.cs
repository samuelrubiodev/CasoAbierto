using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class ElevenLabsHttpRequest : IDaoHttpRequestExtended<JObject, StringContent>
{
    private readonly string urlBase = "https://api.elevenlabs.io/v1/";
    private readonly HttpClient httpClient;

    public ElevenLabsHttpRequest()
    {
        httpClient = new()
        {
            BaseAddress = new Uri(urlBase)

        };
        httpClient.DefaultRequestHeaders.Add("Xi-Api-Key", ApiKey.API_KEY_ELEVENLABS);
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<JObject> GetAsync(string url)
    {
        var response = await httpClient.GetAsync(url);
        string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        return JObject.Parse(body);
    }
}