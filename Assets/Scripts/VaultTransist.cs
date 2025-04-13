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

        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var key = doc.RootElement.GetProperty("data").GetProperty("data").GetProperty(keyname).GetString();
            return key;
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    public async Task<string> EncryptAsync(string keyName, string plainText)
    {
        var payload = new
        {
            plaintext = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText))
        };

        _httpClient.DefaultRequestHeaders.Remove("X-Vault-Token");
        _httpClient.DefaultRequestHeaders.Add("X-Vault-Token", VaultToken);

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{VaultAddress}/v1/transit/encrypt/{keyName}", content);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var cipherText = doc.RootElement.GetProperty("data").GetProperty("ciphertext").GetString();
            return cipherText;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
    }


    public async Task<string> DecryptAsync(string keyName, string cipherText)
    {
        var payload = new { ciphertext = cipherText };

        _httpClient.DefaultRequestHeaders.Remove("X-Vault-Token");
        _httpClient.DefaultRequestHeaders.Add("X-Vault-Token", VaultToken);

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{VaultAddress}/v1/transit/decrypt/{keyName}", content);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var base64Plain = doc.RootElement.GetProperty("data").GetProperty("plaintext").GetString();
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64Plain));
        }
        catch (KeyNotFoundException ex)
        {
            Debug.LogError("KeyNotFoundException: " + ex.Message);
            throw;
        }
    }

    private void LeerConfiguracion()
    {
        Config config = new ("config");
        VaultAddress = config.GetKey("VAULT_IP");
        VaultToken = config.GetKey("VAULT_TOKEN");
    }
}