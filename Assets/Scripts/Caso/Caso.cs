using System;
using System.Collections.Generic;
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

}