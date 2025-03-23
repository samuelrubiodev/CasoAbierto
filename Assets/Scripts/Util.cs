using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using TMPro;
using UnityEngine;
using Utilities.Extensions;

public static class Util
{
    public static JArray ObtenerPersonajes(List<Personaje> personajesLista)
    {
        var personajes = new List<Dictionary<string, string>>();

        foreach (Personaje personaje in personajesLista)
        {
            var personajeDiccionario = new Dictionary<string, string>
            {
                { "nombre", personaje.nombre },
                { "rol", personaje.rol },
                { "estado", personaje.estado },
                { "descripcion", personaje.descripcion },
                { "estado_emocional", personaje.estadoEmocional }
            };

            personajes.Add(personajeDiccionario);
        }
       
        var objeto = JsonConvert.SerializeObject(personajes);
        return JArray.Parse(objeto);
    }

    public static JArray ObtenerEvidencias(List<Evidencia> evidenciasLista)
    {
        var evidencias = new List<Dictionary<string, string>>();

        foreach (Evidencia evidencia in evidenciasLista)
        {
            var evidenciaDiccionario = new Dictionary<string, string>
            {
                { "nombre", evidencia.nombre },
                { "descripcion", evidencia.descripcion },
                { "tipo", evidencia.tipo },
                { "analisis", evidencia.analisis },
                {"ubicacion", evidencia.ubicacion }
            };

            evidencias.Add(evidenciaDiccionario);
        }
        
        var objeto = JsonConvert.SerializeObject(evidencias);
        return JArray.Parse(objeto);
    }

    public static JArray ObtenerCronologias(List<Cronologia> cronologiasLista)
    {
        var cronologias = new List<Dictionary<string, string>>();

        foreach (Cronologia cronologia in cronologiasLista)
        {
            var cronologiaDiccionario = new Dictionary<string, string>
            {
                { "fecha", cronologia.fecha.ToString() },
                { "hora", cronologia.hora },
                { "evento", cronologia.evento }
            };

            cronologias.Add(cronologiaDiccionario);
        }

        var objeto = JsonConvert.SerializeObject(cronologias);
        return JArray.Parse(objeto);
    }

    public static void AddValues(HashEntry[] hashEntries, Dictionary<string, Action<string>> mapeo) {
        foreach (HashEntry hashEntry in hashEntries)
        {
            if (mapeo.TryGetValue(hashEntry.Name, out var setter))
            {
                setter(hashEntry.Value);
            }
        }
    }

    public static async Task<long> GetNewId(string key, HashEntry[] hashes, RedisManager redisManager)
    {
        long id = await Task.Run(() => redisManager.GetNewId(key));
        await Task.Run(() => redisManager.SetHash($"{key}:{id}", hashes));
        return id;
    }

    public static void LoadText(GameObject gameObject, string[] text) {
        for (int i = 0; i < text.Length; i++)
        {
            if (gameObject.transform.GetChild(i).GetComponent<TextMeshProUGUI>() != null)
            {
                gameObject.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = text[i];
            }
        }
    }

    public static void LoadBool(GameObject gameObject, bool[] values) 
    {
        for (int i = 0; i < values.Length; i++)
        {
            gameObject.transform.GetChild(i).SetActive(values[i]);
        }
    }
}