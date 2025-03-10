using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

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
}