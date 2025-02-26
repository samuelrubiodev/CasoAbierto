using System.Collections.Generic;

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
}