using System;
using System.Collections.Generic;
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
}