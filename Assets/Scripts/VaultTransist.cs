using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;

public class VaultTransit
{
    private readonly HttpClient _httpClient = new();
    private string VaultAddress = "";
    private string VaultToken = "";

    public VaultTransit()
    {
        LeerConfiguracion();
    }

    public async Task<string> GetKey(string keyname)
    {
        _httpClient.DefaultRequestHeaders.Remove("X-Vault-Token");
        _httpClient.DefaultRequestHeaders.Add("X-Vault-Token", VaultToken);

        var response = await _httpClient.GetAsync($"{VaultAddress}/v1/secret/data/keys");
        var jsonResponse = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(jsonResponse);
        var key = doc.RootElement.GetProperty("data").GetProperty("data");

        if (!key.TryGetProperty(keyname, out var keyElement))
        {
            throw new KeyNotFoundException($"La clave '{keyname}' no fue encontrada.");
        }

        return keyElement.GetString();
    }

    private void LeerConfiguracion()
    {
        Config config = new ("config");
        VaultAddress = config.GetKey("VAULT_IP");
        VaultToken = config.GetKey("VAULT_TOKEN");
    }
}