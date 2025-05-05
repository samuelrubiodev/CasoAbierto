using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;

public class Caso
{
    public static JObject jsonCaso { get; set; }
    public static Caso caso { get; set; }
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

    public Caso(string idCaso, string lugar, string fechaOcurrido, string dificultad, string tiempoRestante, string tituloCaso, string descripcion, 
        string explicacionCasoResuelto)
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


    public static Caso FromJSONtoObject(JObject json)
    {
        Caso caso = new(
            json["id"].ToString(),
            json["location"].ToString(),
            json["date_occurred"].ToString(),
            json["difficult"].ToString(),
            json["time_remaining"].ToString(),
            json["title"].ToString(),
            json["description"].ToString(),
            json["explanation_case_solved"].ToString()
        );
        caso.personajes = new List<Personaje>();
        caso.evidencias = new List<Evidencia>();
        caso.cronologia = new List<Cronologia>();

        JArray personajes = (JArray)json["characters"];
        foreach (var personaje in personajes)
        {
            JObject personajeJson = JObject.Parse(personaje.ToString());
            Personaje personajeObj = Personaje.FromJSONtoObject(personajeJson);
            caso.personajes.Add(personajeObj);

            JArray arrayMessages = (JArray)json["messages"];

            List<ChatMessage> messages = new()
            {
                new SystemChatMessage(PromptSystem.PROMPT_SYSTEM_CONVERSATION + " " + APIRequest.DATOS_CASO, Role.SYSTEM)
            };

            if (arrayMessages != null)
            {   
                for (int i = 0; i < arrayMessages.Count; i++)
                {
                    JObject message = (JObject)arrayMessages[i];
                    int characterID = int.Parse(message["character_id"].ToString());

                    if (characterID == personajeObj.id)
                    {
                        string role = message["role"].ToString();
                        string messsage = message["message"].ToString();

                        if (role == "user")
                        {
                            messages.Add(new UserChatMessage(messsage));
                        }
                        else if (role == "assistant")
                        {
                            messages.Add(new AssistantChatMessage(messsage));
                        }
                    }
                }
            }
            
            personajeObj.chatMessage = messages;
        }

        JArray evidencias = (JArray)json["evidences"];
        foreach (var evidencia in evidencias)
        {
            JObject evidenciaJson = JObject.Parse(evidencia.ToString());
            Evidencia evidenciaObj = Evidencia.FromJSONtoObject(evidenciaJson);
            caso.evidencias.Add(evidenciaObj);
        }

        JArray cronologias = (JArray)json["timeline"];
        foreach (var cronologia in cronologias)
        {
            JObject cronologiaJson = JObject.Parse(cronologia.ToString());
            Cronologia cronologiaObj = Cronologia.FromJSONtoObject(cronologiaJson);
            caso.cronologia.Add(cronologiaObj);
        }

        return caso;
    }

    public override string ToString()
    {
        return $"Caso: {tituloCaso}, {descripcion}, {dificultad}, {fechaOcurrido}, {lugar}, {tiempoRestante}";
    }
}