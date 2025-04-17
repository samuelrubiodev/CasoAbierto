using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

public class Evidencia
{
    public string nombre { get; set; }
    public string descripcion { get; set; }
    public string analisis { get; set; }
    public string tipo { get; set; }
    public string ubicacion { get; set; }

    public bool analizada { get; set; }

    public Evidencia()
    {
    }

    public Evidencia(string nombre, string descripcion, string analisis, string tipo, string ubicacion)
    {
        this.nombre = nombre;
        this.descripcion = descripcion;
        this.analisis = analisis;
        this.tipo = tipo;
        this.ubicacion = ubicacion;
    }

    public static Evidencia FromJSONtoObject(JObject json)
    {
        Evidencia evidencia = new (
            json["name"].ToString(),
            json["description"].ToString(),
            json["analysis"].ToString(),
            json["type"].ToString(),
            json["location"].ToString()
        );

        return evidencia;
    }

    public static Evidencia GetEvidence(HashEntry[] evidenciaHash) 
    {
        Evidencia evidencia = new();

        var mapeo = new Dictionary<string, Action<string>>
        {
            { "nombre", valor => evidencia.nombre = valor },
            { "descripcion", valor => evidencia.descripcion = valor },
            { "tipo", valor => evidencia.tipo = valor },
            { "analisis", valor => evidencia.analisis = valor },
            { "ubicacion", valor => evidencia.ubicacion = valor }
        };

        Util.AddValues(evidenciaHash, mapeo);
        return evidencia;
    }

    public static async Task SetHashEvidence(JObject respuestaCaso, long jugadorID, long casoID, RedisManager redisManager)
    {
        foreach (JObject evidencia in respuestaCaso["Caso"]?["evidencias"])
        {
            HashEntry[] hashEvidencias = new HashEntry[]
            {
                new ("nombre", evidencia["nombre"].ToString()),
                new ("descripcion", evidencia["descripcion"].ToString()),
                new ("analisis", evidencia["analisis"].ToString()),
                new ("tipo", evidencia["tipo"].ToString()),
                new ("ubicacion", evidencia["ubicacion"].ToString())
            };

            await Util.GetNewId($"jugadores:{jugadorID}:caso:{casoID}:evidencias", hashEvidencias, redisManager);
        }
    }

    public string[] GetSimpleDataStrings()
    {
        return new string[] { nombre, tipo };
    }

    public string[] GetDataStrings() 
    {
        return new string[] { nombre, descripcion, tipo, analizada ? analisis : "No analizada" };
    }


}