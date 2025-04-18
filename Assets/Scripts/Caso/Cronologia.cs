using System;
using Newtonsoft.Json.Linq;

public class Cronologia
{
    public DateTime fecha { get; set; }
    public string hora { get; set; }
    public string evento { get; set; }

    public static Cronologia FromJSONtoObject(JObject json)
    {
        return new Cronologia
        {
            fecha = DateTime.Parse(json["date"].ToString()),
            hora = json["hour"].ToString(),
            evento = json["event"].ToString()
        };
    }
}