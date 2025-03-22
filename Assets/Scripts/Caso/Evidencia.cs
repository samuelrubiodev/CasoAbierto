using System;
using System.Collections.Generic;
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
}