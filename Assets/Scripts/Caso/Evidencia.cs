public class Evidencia
{
    public string nombre { get; set; }
    public string descripcion { get; set; }
    public string analisis { get; set; }
    public string tipo { get; set; }
    public string ubicacion { get; set; }

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
}