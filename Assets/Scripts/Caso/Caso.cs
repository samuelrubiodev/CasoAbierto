using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

public class Caso
{
    public string idCaso { get; set; }
    public string lugar { get; set; }
    public string fechaOcurrido { get; set; }
    public string dificultad { get; set; }
    public string tiempoRestante { get; set; }
    public string tituloCaso { get; set; }
    public string descripcion { get; set; }
    public string explicacionCasoResuelto { get; set; }
    public List<Cronologia> cronologia {get; set;}
    public List<Evidencia> evidencias {get; set;}
    public List<Personaje> personajes {get; set;}

    public Caso()
    {
    }

    public Caso(string idCaso, string lugar, string fechaOcurrido, string dificultad, string tiempoRestante, string tituloCaso, string descripcion, string explicacionCasoResuelto)
    {
        this.idCaso = idCaso;
        this.lugar = lugar;
        this.fechaOcurrido = fechaOcurrido;
        this.dificultad = dificultad;
        this.tiempoRestante = tiempoRestante;
        this.tituloCaso = tituloCaso;
        this.descripcion = descripcion;
        this.explicacionCasoResuelto = explicacionCasoResuelto;
    }

    public static Caso CreateEmptyCaso(string idCaso) {
        Caso caso = new()
        {
            idCaso = idCaso,
            personajes = new List<Personaje>(),
            evidencias = new List<Evidencia>(),
            cronologia = new List<Cronologia>()
        };

        return caso;
    }

    public static void PopulateCasoBasicInfo(HashEntry[] hashCaso, Caso caso) {
        var mapeo = new Dictionary<string, Action<string>>
        {
            { "tituloCaso", valor => caso.tituloCaso = valor },
            { "descripcionCaso", valor => caso.descripcion = valor },
            { "dificultad", valor => caso.dificultad = valor },
            { "fechaOcurrido", valor => caso.fechaOcurrido = valor },
            { "lugar", valor => caso.lugar = valor },
            { "tiempoRestante", valor => caso.tiempoRestante = valor },
            { "explicacionCasoResuelto", valor => caso.explicacionCasoResuelto = valor }
        };

        Util.AddValues(hashCaso, mapeo);
    }

    public static string AddCasoDetails(Jugador jugador) 
    {
        string casosGenerados = "";
        foreach (Caso caso in jugador.casos)
        {
            JObject objetoCaso = new()
            {
                ["Caso"] = new JObject
                {
                    ["tituloCaso"] = caso.tituloCaso,
                    ["descripcionCaso"] = caso.descripcion,
                    ["dificultad"] = caso.dificultad,
                    ["fechaOcurrido"] = caso.fechaOcurrido,
                    ["lugar"] = caso.lugar,
                    ["tiempoRestante"] = caso.tiempoRestante,

                    ["cronologia"] = Util.ObtenerCronologias(caso.cronologia),
                    ["evidencias"] = Util.ObtenerEvidencias(caso.evidencias),
                    ["personajes"] = Util.ObtenerPersonajes(caso.personajes),
                    ["explicacionCasoResuelto"] = caso.explicacionCasoResuelto
                }
            };

            string jsonCaso = JsonConvert.SerializeObject(objetoCaso);
            casosGenerados += jsonCaso + "\n";
        }
        return casosGenerados;
    }

    public static async Task<long> SetHashCaso(JObject respuestaCaso, long jugadorID, RedisManager redisManager) 
    {
        HashEntry[] hashCaso = new HashEntry[]
        {
            new ("tituloCaso", respuestaCaso["Caso"]["tituloCaso"].ToString()),
            new ("descripcionCaso", respuestaCaso["Caso"]["descripcionCaso"].ToString()),
            new ("dificultad", respuestaCaso["Caso"]["dificultad"].ToString()),
            new ("fechaOcurrido", respuestaCaso["Caso"]["fechaOcurrido"].ToString()),
            new ("lugar", respuestaCaso["Caso"]["lugar"].ToString()),
            new ("tiempoRestante", respuestaCaso["Caso"]["tiempoRestante"].ToString()),
            new ("explicacionCasoResuelto", respuestaCaso["Caso"]["explicacionCasoResuelto"].ToString())
        };

        long casoID = await Util.GetNewId($"jugadores:{jugadorID}:caso", hashCaso, redisManager);
        return casoID;
    }
}