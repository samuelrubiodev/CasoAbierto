using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace GroqApiLibrary
{
    public interface ILlmProvider
    {
        Task<string> GenerateAsync(string prompt);
    }


    public class GroqLlmProvider : ILlmProvider, IDisposable
    {
        private readonly GroqApiClient _client;
        private readonly string _model;

        public GroqLlmProvider(string apiKey, string model)
        {
            _client = new GroqApiClient(apiKey, "https://api.groq.com/openai/v1");
            _model = model;
        }

        public async Task<string> GenerateAsync(string prompt)
        {
            var request = new JObject
            {
                ["model"] = _model,
                ["messages"] = new JArray
                {
                    new JObject
                    {
                        ["role"] = "user",
                        ["content"] = prompt
                    }
                }
            };

            var response = await _client.CreateChatCompletionAsync(request);
            return response?["choices"]?[0]?["message"]?["content"]?.ToString() ?? string.Empty;
        }


        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
