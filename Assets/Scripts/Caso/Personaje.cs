using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;
using UnityEngine;

public class Personaje
{
    public int id { get; set; }
    public string nombre { get; set; }
    public string estado { get; set; }
    public string descripcion { get; set; }
    public string estadoEmocional { get; set; }
    public string rol { get; set; }
    public string sexo { get; set; }
    public List<ChatMessage> chatMessage { get; set; }

    public Personaje()
    {
    }

    public Personaje(int id, string nombre, string estado, string descripcion, string estadoEmocional, string rol, string sexo)
    {
        this.id = id;
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
            int.Parse(json["id"].ToString()),
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
            if (personaje.sexo.ToLower() == "masculino")
            {
                List<string> availableMaleImages = new List<string>(ImageConstants.MALE_IMAGES);
                
                while (maleImageIndex < charactersThatHaveGoneOut.Count && 
                       charactersThatHaveGoneOut[maleImageIndex] < availableMaleImages.Count)
                {
                    availableMaleImages.RemoveAt(charactersThatHaveGoneOut[maleImageIndex]);
                    maleImageIndex++;
                }
                
                if (availableMaleImages.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, availableMaleImages.Count);
                    resultImages.Add(availableMaleImages[randomIndex]);
                    
                    charactersThatHaveGoneOut.Add(randomIndex);
                }
                else
                {
                    resultImages.Add(ImageConstants.MALE_IMAGES[0]);
                }
            }
            else
            {
                List<string> availableFemaleImages = new List<string>(ImageConstants.FEMALE_IMAGES);
                
                while (femaleImageIndex < charactersThatHaveGoneOut.Count && 
                    charactersThatHaveGoneOut[femaleImageIndex] < availableFemaleImages.Count)
                {
                    availableFemaleImages.RemoveAt(charactersThatHaveGoneOut[femaleImageIndex]);
                    femaleImageIndex++;
                }
                
                if (availableFemaleImages.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, availableFemaleImages.Count);
                    resultImages.Add(availableFemaleImages[randomIndex]);
                    
                    charactersThatHaveGoneOut.Add(randomIndex);
                }
                else
                {
                    resultImages.Add(ImageConstants.FEMALE_IMAGES[0]);
                }
            }
        }
        
        return resultImages;
    }
}