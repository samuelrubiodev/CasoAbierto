public class Personaje
{
    public string nombre { get; set; }
    public string estado { get; set; }
    public string descripcion { get; set; }
    public string estadoEmocional { get; set; }
    public string rol { get; set; }

    public Personaje()
    {
    }

    public Personaje(string nombre, string estado, string descripcion, string estadoEmocional, string rol)
    {
        this.nombre = nombre;
        this.estado = estado;
        this.descripcion = descripcion;
        this.estadoEmocional = estadoEmocional;
        this.rol = rol;
    }
}