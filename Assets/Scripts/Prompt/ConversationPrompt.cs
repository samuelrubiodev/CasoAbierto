using Newtonsoft.Json.Linq;

public class ConversationPrompt : IPromptMaker<JObject>
{
    public JObject CreatePrompt(string input)
    {
        JObject evidenciaSeleccionada = new ();

        Evidencia evidencia = MenuEvidencias.evidenciaSeleccionada;

        if (MenuEvidencias.evidenciaSeleccionada != null)
        {
            evidenciaSeleccionada = new JObject
            {
                ["nombre"] = evidencia.nombre,
                ["descripcion"] = evidencia.descripcion
            };
        }

        JObject personajeSeleccionado = new();

        Personaje personaje = MenuPersonajes.personajeSeleccionado;

        if (MenuPersonajes.personajeSeleccionado != null)
        {
            personajeSeleccionado = new JObject
            {
                ["nombre"] = personaje.nombre,
                ["rol"] = personaje.rol,
                ["estado"] = personaje.estado,
                ["descripcion"] = personaje.descripcion,
                ["estado_emocional"] = personaje.estadoEmocional
            };
        }

        return new JObject
        {
            ["personajeActual"] = personajeSeleccionado,
            ["evidenciaSeleccionada"] = evidenciaSeleccionada,
            ["mensajes"] = new JObject
            {
                ["mensajeUsuario"] = input,
            }
        };
    }
}