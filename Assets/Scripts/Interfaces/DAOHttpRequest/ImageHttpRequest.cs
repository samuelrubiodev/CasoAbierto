using System;
using System.Net.Http;
using System.Threading.Tasks;

public class ImageHttpRequest : IDaoHttpRequestExtended<byte[], StringContent>
{
    private readonly string urlBase = "http://" + Server.ACTIVE_CASE_HOST;
    private readonly HttpClient httpClient;

    public ImageHttpRequest()
    {
        httpClient = new()
        {
            BaseAddress = new Uri(urlBase)
        };
    }

    public async Task<byte[]> GetAsync(string url)
    {
        var responseImage = await httpClient.GetAsync(urlBase + url);
        if (responseImage.IsSuccessStatusCode)
        {
            return await responseImage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }
        else
        {
            throw new Exception("Error en la solicitud GET a " + urlBase + url);
        }
    }
}