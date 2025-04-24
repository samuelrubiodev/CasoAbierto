using System.Collections.Generic;
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

    public static List<string> GetRandomImages(List<Personaje> personajes, List<int> charactersThatHaveGoneOut)
    {
        List<string> resultImages = new List<string>();
        int maleImageIndex = 0;
        int femaleImageIndex = 0;
        
        foreach (Personaje personaje in personajes)
        {
            // Determine which image set to use based on character gender
            if (personaje.sexo.ToLower() == "masculino")
            {
                List<string> availableMaleImages = new List<string>(ImageConstants.MALE_IMAGES);
                
                // Remove images that have already been used
                while (maleImageIndex < charactersThatHaveGoneOut.Count && 
                       charactersThatHaveGoneOut[maleImageIndex] < availableMaleImages.Count)
                {
                    availableMaleImages.RemoveAt(charactersThatHaveGoneOut[maleImageIndex]);
                    maleImageIndex++;
                }
                
                // Get random image from available ones
                if (availableMaleImages.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, availableMaleImages.Count);
                    resultImages.Add(availableMaleImages[randomIndex]);
                    
                    // Add this image index to the used list
                    charactersThatHaveGoneOut.Add(randomIndex);
                }
                else
                {
                    // Add default male image if all are used
                    resultImages.Add(ImageConstants.MALE_IMAGES[0]);
                }
            }
            else
            {
                List<string> availableFemaleImages = new List<string>(ImageConstants.FEMALE_IMAGES);
                
                // Remove images that have already been used
                while (femaleImageIndex < charactersThatHaveGoneOut.Count && 
                       charactersThatHaveGoneOut[femaleImageIndex] < availableFemaleImages.Count)
                {
                    availableFemaleImages.RemoveAt(charactersThatHaveGoneOut[femaleImageIndex]);
                    femaleImageIndex++;
                }
                
                // Get random image from available ones
                if (availableFemaleImages.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, availableFemaleImages.Count);
                    resultImages.Add(availableFemaleImages[randomIndex]);
                    
                    // Add this image index to the used list
                    charactersThatHaveGoneOut.Add(randomIndex);
                }
                else
                {
                    // Add default female image if all are used
                    resultImages.Add(ImageConstants.FEMALE_IMAGES[0]);
                }
            }
        }
        
        return resultImages;
    }
}