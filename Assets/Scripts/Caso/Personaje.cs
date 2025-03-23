using System;
using System.Collections.Generic;
using StackExchange.Redis;

public class Personaje
{
    public string nombre { get; set; }
    public string estado { get; set; }
    public string descripcion { get; set; }
    public string estadoEmocional { get; set; }
    public string rol { get; set; }
    public string sexo { get; set; }

    public Personaje()
    {
    }

    public Personaje(string nombre, string estado, string descripcion, string estadoEmocional, string rol, string sexo)
    {
        this.nombre = nombre;
        this.estado = estado;
        this.descripcion = descripcion;
        this.estadoEmocional = estadoEmocional;
        this.rol = rol;
        this.sexo = sexo;
    }

    public static Personaje GetPlayer(HashEntry[] personajeHash) 
    {
        Personaje personaje = new();

        var mapeo = new Dictionary<string, Action<string>>
        {
            { "nombre", valor => personaje.nombre = valor },
            { "estado", valor => personaje.estado = valor },
            { "descripcion", valor => personaje.descripcion = valor },
            { "estado_emocional", valor => personaje.estadoEmocional = valor },
            { "rol", valor => personaje.rol = valor },
            { "sexo", valor => personaje.sexo = valor},
        };

        Util.AddValues(personajeHash, mapeo);
        return personaje;
    }

    public string[] GetSimpleDataStrings()
    {
        return new string[] { nombre, estado, estadoEmocional, rol };
    }

    public string[] GetDataStrings() 
    {
        return new string[] { nombre, descripcion, estado, estadoEmocional, rol };
    }
}