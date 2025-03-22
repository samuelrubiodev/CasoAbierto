using System;
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
    public DateTime ultima_conexion { get; set; }
    public List<Caso> casos;

    public Jugador()
    {
    }

    public Jugador(long idJugador, string nombre, string estado, string progreso, DateTime ultima_conexion)
    {
        this.idJugador = idJugador;
        this.nombre = nombre;
        this.estado = estado;
        this.progreso = progreso;
        this.ultima_conexion = ultima_conexion;
    }

    public static HashEntry[] GetHashEntriesJugador(string nombreJugador)
    {
        return new HashEntry[]
        {
            new ("nombre", nombreJugador),
            new ("estado", "inactivo"),
            new ("progreso", "SinCaso"),
            new ("ultima_conexion", DateTime.Now.ToString())
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

    public string toString()
    {
        return "Jugador{" +
                "idJugador=" + idJugador +
                ", nombre='" + nombre + '\'' +
                ", estado='" + estado + '\'' +
                ", progreso='" + progreso + '\'' +
                ", ultima_conexion=" + ultima_conexion +
                ", casos=" + casos +
                '}';
    }
}