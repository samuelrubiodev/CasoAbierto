using Newtonsoft.Json.Linq;

public class ConversationPrompt : IPromptMaker<JObject>
{
    public JObject CreatePrompt(string input)
    {
        JObject evidenciaSeleccionada = new ();
        JObject personajeSeleccionado = new();

        Evidencia evidencia = RadialUIController.selectedEvidence;
        Personaje personaje = MenuPersonajes.personajeSeleccionado;

        if (evidencia != null)
        {
            evidenciaSeleccionada = new JObject
            {
                ["nombre"] = evidencia.nombre,
                ["descripcion"] = evidencia.descripcion,
                ["analisis"] = evidencia.analisis
            };
        }

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