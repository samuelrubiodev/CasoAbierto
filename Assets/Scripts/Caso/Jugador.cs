using System;
using System.Collections.Generic;
using StackExchange.Redis;
using Utilities.Rest.Extensions;

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