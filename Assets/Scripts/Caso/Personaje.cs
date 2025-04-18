using Newtonsoft.Json.Linq;

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

    public static Personaje FromJSONtoObject(JObject json)
    {
        Personaje personaje = new (
            json["name"].ToString(),
            json["state"].ToString(),
            json["description"].ToString(),
            json["state_emotional"].ToString(),
            json["role"].ToString(),
            json["genre"].ToString()
        );

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