public class Schemas {
    public const string CONVERSATIONS = @"
        {
            ""type"": ""object"",
            ""properties"": {
                ""haGanadoUsuario"": {
                ""type"": ""boolean"",
                ""description"": ""Indica si el jugador ha resuelto con éxito el caso respecto a este sospechoso (descubierto la verdad: culpable o inocente)""
                },
                ""razonamiento"": {
                ""type"": ""string"",
                ""description"": ""Explicación concisa (1-2 frases) del motivo de las decisiones tomadas para seHaTerminado y haGanadoUsuario.""
                }
            },
            ""required"": [""haGanadoUsuario"", ""razonamiento""],
            ""additionalProperties"": false
        }";

    public const string EVIDENCES = @"
        {
            ""type"": ""object"",
            ""properties"": {
                ""evidencia"": {
                    ""type"": ""object"",
                    ""properties"": {
                        ""evidenciaSeleccionada"": { ""type"": ""string"", ""description"": ""Nombre de la evidencia seleccionada"" },
                        ""tipoAnalisis"": { ""type"": ""string"", ""description"": ""Tipo de analisis a realizar, huellas daactilares por ejemplo"" },
                        ""resultadoAnalisis"": { ""type"": ""string"", ""description"": ""Analisis de la evidencia"" }
                    },
                    ""required"": [""evidenciaSeleccionada"",""tipoAnalisis"",""resultadoAnalisis""],
                    ""additionalProperties"": false
                }
            },
            ""required"": [""evidencia""],
            ""additionalProperties"": false
        }";

    public const string CHARACTER_EMOTIONAL_STATE = @"
        {
            ""type"": ""object"",
            ""properties"": {
                ""personajeSeleccionado"": {
                    ""type"": ""object"",
                    ""properties"": {
                        ""nombre"": { ""type"": ""string"", ""description"": ""Nombre del personaje"" },
                        ""estado_emocional"": { ""type"": ""string"", ""description"": ""Estado emocional del personaje"" }
                    },
                    ""required"": [""nombre"",""estado_emocional""],
                    ""additionalProperties"": false
                }
            },
            ""required"": [""personajeSeleccionado""],
            ""additionalProperties"": false
        }";
} 