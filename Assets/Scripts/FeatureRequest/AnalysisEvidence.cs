using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI.Chat;
using UnityEngine;

public class AnalysisEvidence : IFeatureRequest<string>
{
    private string openRouterApiKey;
    public AnalysisEvidence()
    {
        openRouterApiKey = ApiKey.API_KEY_OPEN_ROUTER;
    }

    public async Task<string> SendRequest(string prompt)
    {
        try {
            string jsonSchema = Schemas.EVIDENCES;

            List<ChatMessage> mensajes = new()
            {
                new SystemChatMessage(PromptSystem.PROMPT_SYSTEM_ANALYSIS_EVIDENCE + APIRequest.DATOS_CASO),
                new UserChatMessage(prompt)
            };

            ChatManager chatManager = new (openRouterApiKey, mensajes);
            ChatCompletionOptions options = chatManager.CreateChatCompletionOptions(jsonSchema);
            AsyncCollectionResult<StreamingChatCompletionUpdate> completionResult = chatManager.CreateStremingChat(ChatManager.CHAT_MODEL, options);

            string message = "";
            await foreach (StreamingChatCompletionUpdate update in completionResult)
            {
                if (update.ContentUpdate.Count > 0)
                {
                    string texto = update.ContentUpdate[0].Text;
                    message += texto;
                }
            }

            APIRequest.characters[SelectionCharacters.selectedCharacter.id].chatMessage.Add(new UserChatMessage("El investigador ha analizado esta evidencia y se ha concluido lo siguiente:" + message));

            return message;
        } catch (Exception e) {
            Debug.LogError(e.Message);
            throw;
        }
    }
}