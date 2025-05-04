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

        var jsonData = new
        {
            message = prompt,
            role = Role.USER,
            characterID = APIRequest.characters[SelectionCharacters.selectedCharacter.id].id.ToString(),
        };

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(jsonData),
            System.Text.Encoding.UTF8,
            "application/json");

        await caseHttpRequest.PostAsync("/players/" + Jugador.jugador.idJugador + "/case/" + Caso.caso.idCaso + "/message", jsonContent);

        APIRequest.characters[SelectionCharacters.selectedCharacter.id].chatMessage.Add(new UserChatMessage(prompt));

        try
        {  
            ChatManager chatManager = new (openRouterApiKey, APIRequest.characters[SelectionCharacters.selectedCharacter.id].chatMessage);
            ChatCompletionOptions options = chatManager.CreateChatCompletionOptions();
            AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = chatManager.CreateStremingChat(ChatManager.CHAT_MODEL, options);

            StringBuilder messageCharacterBuilder = new();
            string completionIdBuilder = "";
            await foreach (StreamingChatCompletionUpdate update in completionUpdates)
            {
                completionIdBuilder = update.CompletionId.ToString();
                if (update.ContentUpdate.Count > 0) messageCharacterBuilder.Append(update.ContentUpdate[0].Text);
            }
            string message = messageCharacterBuilder.ToString();

            jsonData = new
            {
                message = message,
                role = Role.ASSISTANT,
                characterID = APIRequest.characters[SelectionCharacters.selectedCharacter.id].id.ToString(),
            };

            jsonContent = new StringContent(
                JsonConvert.SerializeObject(jsonData),
                System.Text.Encoding.UTF8,
                "application/json");

            await caseHttpRequest.PostAsync("/players/" + Jugador.jugador.idJugador + "/case/" + Caso.caso.idCaso + "/message", jsonContent);

            GenerationIDHttpRequest generationIDHttpRequest = new ();
            JObject jsonResponse = await generationIDHttpRequest.GetAsync($"generation?id={completionIdBuilder.ToString()}");
            APICreditsManager.jsonOpenRouterResponse = jsonResponse;

            APIRequest.characters[SelectionCharacters.selectedCharacter.id].chatMessage.Add(new AssistantChatMessage(message));

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