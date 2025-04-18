using Newtonsoft.Json.Linq;

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

    public static Evidencia FromJSONtoObject(JObject json)
    {
        Evidencia evidencia = new (
            json["name"].ToString(),
            json["description"].ToString(),
            json["analysis"].ToString(),
            json["type"].ToString(),
            json["location"].ToString()
        );

        return evidencia;
    }

    public string[] GetSimpleDataStrings()
    {
        return new string[] { nombre, tipo };
    }

    public string[] GetDataStrings() 
    {
        return new string[] { nombre, descripcion, tipo, analizada ? analisis : "No analizada" };
    }


}