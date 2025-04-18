using System.Collections.Generic;
using OpenAI.Chat;

public class Role {

    public const string USER = "user";
    public const string ASSISTANT = "assistant";
    public const string SYSTEM = "system";
}

public class Message
{
    public int id { get; set; }
    public string message { get; set; }
    public string role { get; set; }
    public Dictionary<string, ChatMessage> roles { get; set; } = new Dictionary<string, ChatMessage>();

    public Message(int id, string message, string role)
    {
        this.id = id;
        this.message = message;
        this.role = role;

        roles = new Dictionary<string, ChatMessage>
        {
            { "user", new UserChatMessage(message) },
            { "assistant", new AssistantChatMessage(message) },
            { "system", new SystemChatMessage(message) }
        };
    }

    public static List<ChatMessage> ToChatManagerList(List<Message> messages)
    {
        List<ChatMessage> chatMessages = new List<ChatMessage>();
        foreach (var message in messages)
        {
            if (message.roles.ContainsKey(message.role))
            {
                chatMessages.Add(message.roles[message.role]);
            }
        }
        return chatMessages;

    }

    public ChatMessage ToChatManager()
    {
        return roles.ContainsKey(role) ? roles[role] : null;
    }

}