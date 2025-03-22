using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

public class Cronologia
{
    public DateTime fecha { get; set; }
    public string hora { get; set; }
    public string evento { get; set; }


    public static Cronologia GetTimeline(HashEntry[] cronologiaHash) 
    {
        Cronologia cronologia = new();

        var mapeo = new Dictionary<string, Action<string>>
        {
            { "fecha", valor => cronologia.fecha = DateTime.Parse(valor) },
            { "evento", valor => cronologia.evento = valor },
            { "hora", valor => cronologia.hora = valor }
        };

        Util.AddValues(cronologiaHash, mapeo);

        return cronologia;
    }

    public static async Task SetHashTimeline(JObject respuestaCaso, long jugadorID, long casoID, RedisManager redisManager)
    {
        foreach (JObject cronologia in respuestaCaso["Caso"]["cronologia"])
        {
            HashEntry[] hashCronologia = new HashEntry[]
            {
                new ("fecha", cronologia["fecha"].ToString()),
                new ("hora", cronologia["hora"].ToString()),
                new ("evento", cronologia["evento"].ToString())
            };

            await Util.GetNewId($"jugadores:{jugadorID}:caso:{casoID}:cronologia", hashCronologia, redisManager);
        }
    }
}