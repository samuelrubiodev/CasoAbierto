using System;
using System.ClientModel;
using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;

#nullable enable
public class ChatManager 
{
    private readonly OpenAIClientOptions OpenAIClientOptions;
    private readonly OpenAIClient Client;
    private readonly List<ChatMessage> ChatMessages;

    public ChatManager(string apiKeyOpenRouter, List<ChatMessage> chatMessages)
    {
        OpenAIClientOptions = new()
        {
            Endpoint = new Uri("https://openrouter.ai/api/v1")
        };

        Client = new(new ApiKeyCredential(apiKeyOpenRouter), OpenAIClientOptions);
        ChatMessages = chatMessages;
    }

    public ChatCompletionOptions CreateChatCompletionOptions(string? jsonSchema = null, int maxTokens = 8192, float temperature = 0.7f)
    {
        ChatCompletionOptions options = new()
        {
            MaxOutputTokenCount = maxTokens,
            Temperature = temperature
        };

        if (jsonSchema != null)
        {
            options.ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat (
                jsonSchemaFormatName: "data",
                jsonSchema: BinaryData.FromString(jsonSchema),
                jsonSchemaIsStrict: true);
        }
        return options;
    }

    public AsyncCollectionResult<StreamingChatCompletionUpdate> CreateStremingChat(string model, ChatCompletionOptions options)
    {
        AsyncCollectionResult<StreamingChatCompletionUpdate> completionResult = Client
            .GetChatClient(model)
            .CompleteChatStreamingAsync(ChatMessages, options);

        return completionResult;
    }

    public ChatCompletion CreateChat(string model, ChatCompletionOptions options)
    {
        return Client.GetChatClient(model).CompleteChat(ChatMessages, options);
    }
}