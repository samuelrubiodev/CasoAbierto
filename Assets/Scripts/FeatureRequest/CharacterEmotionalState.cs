using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;
using UnityEngine;

public class CharacterEmotionalState : IFeatureRequest<JObject>
{
    private string openRouterApiKey;
    
    public CharacterEmotionalState()
    {
        openRouterApiKey = ApiKey.API_KEY_OPEN_ROUTER;
    }

    public Task<JObject> SendRequest(string prompt)
    {
        try {
            string jsonSchema = Schemas.CHARACTER_EMOTIONAL_STATE;
            string conversacion = "";

            foreach (var message in APIRequest.chatMessages)
            {
                if (message is UserChatMessage userMessage)
                {
                    conversacion += "Usuario: " + userMessage.Content[0].Text + "\n";
                }
                else if (message is AssistantChatMessage aiMessage)
                {
                    conversacion += "NPC: " + aiMessage.Content[0].Text + "\n";
                }
            }

            List<ChatMessage> mensajes = new() 
            {
                new SystemChatMessage(PromptSystem.PROMPT_SYSTEM_CHARACTER_EMOTIONAL_STATE),
                new UserChatMessage(prompt + conversacion)
            };

            ChatManager chatManager = new (openRouterApiKey, APIRequest.chatMessages);
            ChatCompletionOptions options = chatManager.CreateChatCompletionOptions(jsonSchema);
            ChatCompletion completion = chatManager.CreateChat(ChatManager.CHAT_MODEL, options);

            return Task.FromResult(JObject.Parse(completion.Content[0].Text));
        } catch(Exception ex) {
            Debug.LogError(ex);
            throw;
        }
    }
}
