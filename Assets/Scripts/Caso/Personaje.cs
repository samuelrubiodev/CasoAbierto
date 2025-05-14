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
        List<string> resultImages = new ();
        List<string> availableMaleImages = new (ImageConstants.MALE_IMAGES);
        List<string> availableFemaleImages = new (ImageConstants.FEMALE_IMAGES);
        charactersThatHaveGoneOut.Clear();

        foreach (Personaje personaje in personajes)
        {
            string selectedImage = null;
            int originalIndexOfSelectedImage = -1;

            string sexoPersonaje = personaje.sexo.ToLowerInvariant();

            if (sexoPersonaje == "masculino")
            {
                if (availableMaleImages.Count > 0)
                {
                    int randomIndexInAvailableList = UnityEngine.Random.Range(0, availableMaleImages.Count);
                    selectedImage = availableMaleImages[randomIndexInAvailableList];
                    
                    originalIndexOfSelectedImage = ImageConstants.MALE_IMAGES.IndexOf(selectedImage);
                    
                    availableMaleImages.RemoveAt(randomIndexInAvailableList);
                }
                else
                {
                    selectedImage = ImageConstants.DEFAULT_FEMALE_IMAGE;
                    if (ImageConstants.MALE_IMAGES.Contains(selectedImage))
                    {
                        originalIndexOfSelectedImage = ImageConstants.MALE_IMAGES.IndexOf(selectedImage);
                    }
                }
            }
            else
            {
                if (availableFemaleImages.Count > 0)
                {
                    int randomIndexInAvailableList = UnityEngine.Random.Range(0, availableFemaleImages.Count);
                    selectedImage = availableFemaleImages[randomIndexInAvailableList];
                    
                    originalIndexOfSelectedImage = ImageConstants.FEMALE_IMAGES.IndexOf(selectedImage);
                    
                    availableFemaleImages.RemoveAt(randomIndexInAvailableList);
                }
                else
                {
                    selectedImage = ImageConstants.DEFAULT_MALE_IMAGE;
                    if (ImageConstants.FEMALE_IMAGES.Contains(selectedImage))
                    {
                        originalIndexOfSelectedImage = ImageConstants.FEMALE_IMAGES.IndexOf(selectedImage);
                    }
                }
            }

            resultImages.Add(selectedImage);
            charactersThatHaveGoneOut.Add(originalIndexOfSelectedImage);
        }
        
        return resultImages;
    }
}