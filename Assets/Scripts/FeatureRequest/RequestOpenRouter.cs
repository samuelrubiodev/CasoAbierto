using System;
using UnityEngine;
using OpenAI.Chat;
using System.ClientModel;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FeatureRequest {
    public class RequestOpenRouter : IFeatureRequest<string> {
    private string openRouterApiKey;
    public static string DATOS_CASO = "";

    public RequestOpenRouter() {
        openRouterApiKey = ApiKey.API_KEY_OPEN_ROUTER;
    }

    public async Task<string> SendRequest(string prompt)
    {
        CaseHttpRequest caseHttpRequest = new ();

        if (!APIRequest.chatMessages.Any(x => x is SystemChatMessage ))
        {
            APIRequest.chatMessages.Add(new SystemChatMessage(PromptSystem.PROMPT_SYSTEM_CONVERSATION + " " + APIRequest.DATOS_CASO,Role.SYSTEM));
        }

        var jsonData = new
        {
            message = prompt,
            role = Role.USER
        };

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(jsonData),
            System.Text.Encoding.UTF8,
            "application/json");

        await caseHttpRequest.PostAsync("/players/" + Jugador.jugador.idJugador + "/case/" + Caso.caso.idCaso + "/message", jsonContent);

        APIRequest.chatMessages.Add(new UserChatMessage(prompt));

        try
        {  
            ChatManager chatManager = new (openRouterApiKey, APIRequest.chatMessages);
            ChatCompletionOptions options = chatManager.CreateChatCompletionOptions();
            AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = chatManager.CreateStremingChat(ChatManager.CHAT_MODEL, options);

            StringBuilder messageCharacterBuilder = new();
            string completionIdBuilder = "";
            await foreach (StreamingChatCompletionUpdate update in completionUpdates)
            {
                completionIdBuilder = update.CompletionId.ToString();
                Debug.Log("ID DE LA RESPUESTA: " + completionIdBuilder.ToString());
                if (update.ContentUpdate.Count > 0) messageCharacterBuilder.Append(update.ContentUpdate[0].Text);
            }
            string message = messageCharacterBuilder.ToString();

            jsonData = new
            {
                message = message,
                role = Role.ASSISTANT
            };

            jsonContent = new StringContent(
                JsonConvert.SerializeObject(jsonData),
                System.Text.Encoding.UTF8,
                "application/json");

            await caseHttpRequest.PostAsync("/players/" + Jugador.jugador.idJugador + "/case/" + Caso.caso.idCaso + "/message", jsonContent);

            GenerationIDHttpRequest generationIDHttpRequest = new ();

            JObject jsonResponse = await generationIDHttpRequest.GetAsync($"generation?id={completionIdBuilder.ToString()}");

            OpenRouterImpl openRouterImpl = OpenRouterImpl.Instance;
            await openRouterImpl.UpdateCreditsBalance(await openRouterImpl.GetCostRequest(jsonResponse));
            
            APIRequest.chatMessages.Add(new AssistantChatMessage(message));

            return message;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            throw;
        }
    }
}
}