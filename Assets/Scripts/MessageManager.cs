using GroqApiLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

/**
 * Clase que se encarga de la gesti�n de los mensajes de la conversaci�n
 * 
 */
public class MessageManager
{
    /**
     * Mensajes de la conversaci�n
     */
    private JArray messages;

    public MessageManager()
    {
        messages = new JArray();
    }

    /**
     * Agrega un mensaje a la conversaci�n
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
     * Agrega parametros de la petici�n
     * 
     * @param model: Modelo de IA a utilizar
     * @param temperature: Temperatura de la IA
     * @param maxTokens: M�ximo de tokens a generar
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
     * Obtiene los mensajes de la conversaci�n
     * 
     * @return JArray: Mensajes de la conversaci�n
     */
    public JArray GetMessages()
    {
        return messages;
    }

    /**
     * Limpia los mensajes de la conversaci�n
     */
    public void ClearMessages()
    {
        messages.Clear();
    }
}
