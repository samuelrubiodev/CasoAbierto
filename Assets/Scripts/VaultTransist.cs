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
    private static TextAsset config;

    public static void CargarConfiguracion()
    {
        config = Resources.Load<TextAsset>("config");
    }

    public VaultTransit()
    {
        LeerConfiguracion();
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
        if (config != null)
        {
            foreach (string line in config.text.Split('\n'))
            {
                if (line.Contains("VAULT_IP=")) VaultAddress = line.Split('=')[1].Trim();
                if (line.Contains("VAULT_TOKEN=")) VaultToken = line.Split('=')[1].Trim();
            }
        }
    }
}