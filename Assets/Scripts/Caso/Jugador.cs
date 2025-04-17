using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

public class Jugador
{
    public static Jugador jugador { get; set; }
    public static int indexCaso { get; set; }
    public long idJugador { get; set; }
    public string nombre { get; set; }
    public string estado { get; set; }
    public string progreso { get; set; }
    public List<Caso> casos;

    public Jugador()
    {
    }

    public Jugador(long idJugador, string nombre, string estado, string progreso)
    {
        this.idJugador = idJugador;
        this.nombre = nombre;
        this.estado = estado;
        this.progreso = progreso;
    }

    public static Jugador FromJSONtoObject(JObject json)
    {
        Jugador jugador = new ();
        JObject player = (JObject)json["player"];

        jugador.idJugador = (long)player["id"];
        jugador.nombre = player["name"].ToString();
        jugador.estado = player["state"].ToString();
        jugador.progreso = player["progress"].ToString();

        jugador.casos = new List<Caso>();

        JArray casos = (JArray)json["cases"];
        foreach (var caso in casos)
        {
            JObject casoJson = JObject.Parse(caso.ToString());
            Caso casoObj = Caso.FromJSONtoObject(casoJson);
            jugador.casos.Add(casoObj);
        }
        return jugador;
    }

    public static HashEntry[] GetHashEntriesJugador(string nombreJugador)
    {
        return new HashEntry[]
        {
            new ("nombre", nombreJugador),
            new ("estado", "inactivo"),
            new ("progreso", "SinCaso"),
        };
    }

    public static async Task SetHashPlayer(JObject respuestaCaso, long jugadorID, long casoID, RedisManager redisManager)
    {
        foreach (JObject personaje in respuestaCaso["Caso"]?["personajes"])
        {
            HashEntry[] hashPersonajes = new HashEntry[]
            {
                new ("nombre", personaje["nombre"].ToString()),
                new ("rol", personaje["rol"].ToString()),
                new ("descripcion",personaje["descripcion"].ToString()),
                new ("estado", personaje["estado"].ToString()),
                new ("estado_emocional", personaje["estado_emocional"].ToString()),
                new ("sexo", personaje["sexo"].ToString())
            };

            await Util.GetNewId($"jugadores:{jugadorID}:caso:{casoID}:personajes", hashPersonajes, redisManager);
        }
    }

    public override string ToString()
    {
        return "Jugador{" +
                "idJugador=" + idJugador +
                ", nombre='" + nombre + '\'' +
                ", estado='" + estado + '\'' +
                ", progreso='" + progreso + '\'' +
                ", casos=" + casos +
                '}';
    }
}