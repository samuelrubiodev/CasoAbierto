using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;
using UnityEngine;

public class GameStatus : IFeatureRequest<JObject>
{
    private string openRouterApiKey;
    
    public GameStatus()
    {
        openRouterApiKey = ApiKey.API_KEY_OPEN_ROUTER;
    }

    public Task<JObject> SendRequest(string prompt)
    {
        try
        {
            string jsonSchema = Schemas.CONVERSATIONS;
            string conversacion = "";
            foreach (var message in APIRequest.characters[SelectionCharacters.selectedCharacter.id].chatMessage)
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
                new SystemChatMessage(PromptSystem.PROMPT_SYSTEM_ANALYSIS),
                new UserChatMessage(prompt + conversacion)
            };

            ChatManager chatManager = new (openRouterApiKey, mensajes);
            ChatCompletionOptions options = chatManager.CreateChatCompletionOptions(jsonSchema);
            ChatCompletion completion = chatManager.CreateChat(ChatManager.CHAT_MODEL, options);

            return Task.FromResult(JObject.Parse(completion.Content[0].Text));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            throw;
        }
    }
}