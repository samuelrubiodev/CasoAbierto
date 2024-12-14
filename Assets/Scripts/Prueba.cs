using GroqApiLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using UnityEngine;


public class Prueba : MonoBehaviour
{

    public string apiKey;
    async void Start()
    {

        try
        {
            GroqApiClient groqApiClient = new GroqApiClient(apiKey);

            var request = new JObject
            {
                ["temperature"] = 1,
                ["max_tokens"] = 1024,
                ["model"] = "llama-3.3-70b-specdec",
                ["messages"] = new JArray
                {
                    new JObject
                    {
                        ["role"] = "system",
                        ["content"] = "Eres un asistente muy útil"
                    },
                    new JObject
                    {
                        ["role"] = "user",
                        ["content"] = "¡Hola! ¿Cómo estás?"
                    }
                }
            };


            await foreach (var chunk in groqApiClient.CreateChatCompletionStreamAsync(request))
            {
                var delta = chunk?["choices"]?[0]?["delta"]?["content"]?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(delta))
                {
                    Debug.Log(delta);
                }
            }

        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"API request failed: {e.Message}");
        }
        catch (System.Text.Json.JsonException e)
        {
            Console.WriteLine($"Failed to parse API response: {e.Message}");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
