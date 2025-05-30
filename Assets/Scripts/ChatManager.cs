using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
    public const string CHAT_MODEL = "google/gemini-2.0-flash-001";

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

    public async Task<ClientResult<GeneratedImage>> GeneratedImageAsync(string model, string prompt, ImageGenerationOptions options)
    {
        return await Client.GetImageClient(model).GenerateImageAsync(prompt, options);
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

    public async Task<string> SendMessageAsync(string model, ChatCompletionOptions? options = null) {
        try {
            options ??= CreateChatCompletionOptions();
            AsyncCollectionResult<StreamingChatCompletionUpdate> completionResult = CreateStremingChat(model, options);

            StringBuilder buffer = new();
            await foreach (StreamingChatCompletionUpdate update in completionResult) {
                if (update.ContentUpdate.Count > 0) buffer.Append(update.ContentUpdate[0].Text);       
            }
            
            return buffer.ToString();
        } catch (Exception ex) {
            Console.WriteLine($"Error sending message: {ex.Message}");
            throw;
        }
    }

    public async Task<GeneratedImage> GetImageAsync(string model, string prompt, ImageGenerationOptions options)
    {
        return await GeneratedImageAsync(model, prompt, options);
    }
}