using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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