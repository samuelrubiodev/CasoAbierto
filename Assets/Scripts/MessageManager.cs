using GroqApiLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

/**
 * Clase que se encarga de la gestión de los mensajes de la conversación
 * 
 */
public class MessageManager
{
    /**
     * Mensajes de la conversación
     */
    private JArray messages;

    public MessageManager()
    {
        messages = new JArray();
    }

    /**
     * Agrega un mensaje a la conversación
     * 
     * @param role: Rol del mensaje
     * @param content: Contenido del mensaje
     */
    public void AddMessage(string role, string content)
    {
        messages.Add(new JObject
        {
            ["role"] = role,
            ["content"] = content
        });
    }


    /**
     * Agrega parametros de la petición
     * 
     * @param model: Modelo de IA a utilizar
     * @param temperature: Temperatura de la IA
     * @param maxTokens: Máximo de tokens a generar
     */
    public JObject AgregarRequest(String model, int temperature, int maxTokens)
    {
        var request = new JObject
        {
            ["temperature"] = temperature,
            ["max_tokens"] = maxTokens,
            ["model"] = model,
            ["messages"] = GetMessages()
        };

        return request;
    }

    /**
     * Obtiene los mensajes de la conversación
     * 
     * @return JArray: Mensajes de la conversación
     */
    public JArray GetMessages()
    {
        return messages;
    }

    /**
     * Limpia los mensajes de la conversación
     */
    public void ClearMessages()
    {
        messages.Clear();
    }
}
