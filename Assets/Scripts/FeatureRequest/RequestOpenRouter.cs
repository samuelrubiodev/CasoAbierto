using System;
using UnityEngine;
using OpenAI.Chat;
using System.ClientModel;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using TMPro;

namespace FeatureRequest {
    public class RequestOpenRouter : IFeatureRequest<string> {
    private string openRouterApiKey;
    public static string DATOS_CASO = "";

    private TMP_Text textSubtitle;

    public RequestOpenRouter(TMP_Text textSubtitle) {
        openRouterApiKey = ApiKey.API_KEY_OPEN_ROUTER;
        this.textSubtitle = textSubtitle;
    }

    public async Task<string> SendRequest(string prompt)
    {
        if (!APIRequest.chatMessages.Any(x => x is SystemChatMessage))
        {
            APIRequest.chatMessages.Add(new SystemChatMessage(PromptSystem.PROMPT_SYSTEM_CONVERSATION + " " + APIRequest.DATOS_CASO));
        }

        APIRequest.chatMessages.Add(new UserChatMessage(prompt));

        try
        {  
            ChatManager chatManager = new (openRouterApiKey, APIRequest.chatMessages);
            ChatCompletionOptions options = chatManager.CreateChatCompletionOptions();
            AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = chatManager.CreateStremingChat(ChatManager.CHAT_MODEL, options);

            StringBuilder messageCharacterBuilder = new();
            await foreach (StreamingChatCompletionUpdate update in completionUpdates)
            {
                if (update.ContentUpdate.Count > 0) messageCharacterBuilder.Append(update.ContentUpdate[0].Text);
            }
            string message = messageCharacterBuilder.ToString();
            
            APIRequest.chatMessages.Add(new AssistantChatMessage(message));

            return message;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            throw;
        }
    }
}
}