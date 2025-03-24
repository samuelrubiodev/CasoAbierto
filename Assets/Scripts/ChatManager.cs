using System;
using System.ClientModel;
using System.Collections.Generic;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;

#nullable enable
public class ChatManager 
{
    private readonly OpenAIClientOptions OpenAIClientOptions;
    private readonly OpenAIClient Client;
    private readonly List<ChatMessage> ChatMessages;
    public const string DEFAULT_API_URL = "https://openrouter.ai/api/v1";
    public const string TOGETHER_AI_API_URL = "https://api.together.xyz/v1/";
    
    public const string IMAGE_MODEL_FREE = "black-forest-labs/FLUX.1-schnell-Free";
    public const string CHAT_MODEL = "google/gemini-2.0-flash-001";
    public const string CHAT_MODEL_FREE = "google/gemini-2.0-flash-exp:free";

    public ChatManager(string apiKey, List<ChatMessage> chatMessages)
    {
        OpenAIClientOptions = new()
        {
            Endpoint = new Uri(DEFAULT_API_URL)
        };

        Client = new(new ApiKeyCredential(apiKey), OpenAIClientOptions);
        ChatMessages = chatMessages;
    }

    public ChatManager(string apiKey, string apiUrl)
    {
        OpenAIClientOptions = new()
        {
            Endpoint = new Uri(apiUrl)
        };

        Client = new(new ApiKeyCredential(apiKey), OpenAIClientOptions);
        ChatMessages = new();
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


    public ImageGenerationOptions CreateImageGenerationOptions(GeneratedImageSize size,GeneratedImageFormat format)
    {
        ImageGenerationOptions options = new()
        {
            Size = size,
            ResponseFormat = format
        };

        return options;
    }

    public GeneratedImage GenerateImage(string model, string prompt, ImageGenerationOptions options)
    {
        return Client.GetImageClient(model).GenerateImage(prompt, options);
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