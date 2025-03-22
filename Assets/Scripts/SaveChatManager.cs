using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Chat;
using StackExchange.Redis;
using UnityEngine;

public class SaveChatManager : MonoBehaviour
{
    RedisManager redisManager;
    private CancellationTokenSource cts;
    private List<ChatMessage> chatMessages;
    private ControllerGame controllerGame;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        redisManager = await RedisManager.GetRedisManager();
        cts = new CancellationTokenSource();
        controllerGame = GetComponent<ControllerGame>();
        _ = SaveChat(cts.Token);
    }

    private void OnDestroy()
    {
        cts.Cancel();
    }

    private async Task SaveChat(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(10000, token);

                if (APIRequest.chatMessages == null || APIRequest.chatMessages.Count == 0)
                    continue;

                chatMessages = new List<ChatMessage>(APIRequest.chatMessages);
                IDatabase db = redisManager.GetDB(); 

                int indexCaso = Jugador.indexCaso + 1;

                foreach (ChatMessage chatMessage in chatMessages)
                {
                    string messageId = GenerateMessageId(chatMessage);
                    string redisKey = $"jugadores:{Jugador.jugador.idJugador}:caso:{indexCaso}:mensajes:guardados";

                    bool exists = await db.SetContainsAsync(redisKey, messageId);
                    if (exists)
                        continue; 

                    if (chatMessage is UserChatMessage || chatMessage is AssistantChatMessage)
                    {
                        HashEntry[] hashMessages = new HashEntry[]
                        {
                            new ("role", chatMessage is UserChatMessage ? "user" : "assistant"),
                            new ("message", chatMessage.Content[0].Text)
                        };

                        long chatId = redisManager.GetNewId($"jugadores:{Jugador.jugador.idJugador}:caso:{indexCaso}:mensajes");
                        redisManager.SetHash($"jugadores:{Jugador.jugador.idJugador}:caso:{indexCaso}:mensajes:{chatId}", hashMessages);

                        await db.SetAddAsync(redisKey, messageId);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error en SaveChat: {ex.Message}");
            }
        }
    }

    private string GenerateMessageId(ChatMessage chatMessage)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string rawData = (chatMessage is UserChatMessage ? "user" : "assistant") + chatMessage.Content[0].Text;
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }
    }

    void Update()
    {
        
    }

}