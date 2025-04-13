public class Schemas {
    public const string GENERATION_CASE = @"
        {
            ""type"": ""object"",
            ""properties"": {
                ""datosJugador"": {
                    ""type"": ""object"",
                    ""properties"": {
                            ""nombre"": { ""type"": ""string"", ""description"": ""Nombre del jugador"" },
                            ""estado"": { ""type"": ""string"", ""description"": ""Estado del jugador, Activo o Inactivo"" },
                            ""progreso"": { ""type"": ""string"", ""description"": ""En que caso va, nombre del caso"" }
                        },
                        ""required"": [""nombre"",""estado"",""progreso""],
                        ""additionalProperties"": false
                },
                ""Caso"": {
                    ""type"": ""object"",
                    ""properties"": {
                        ""tituloCaso"": { ""type"": ""string"", ""description"": ""Titulo del caso"" },
                        ""descripcionCaso"": { ""type"": ""string"", ""description"": ""Descripción del caso"" },
                        ""dificultad"": { ""type"": ""string"", ""description"": ""Facil, Medio o Dificil"" },
                        ""fechaOcurrido"": { ""type"": ""string"", ""description"": ""YYYY-MM-DD"" },
                        ""lugar"": { ""type"": ""string"", ""description"": ""Lugar en el que ha ocurrido el caso"" },
                        ""tiempoRestante"": { ""type"": ""string"", ""description"": ""Minutos restantes, solo en formato: MM"" },
                        ""cronologia"": { ""type"": ""array"", ""items"": {
                                ""type"": ""object"",
                                    ""properties"": {
                                        ""fecha"": {""type"": ""string"", ""description"": ""YYYY-MM-DD""},
                                        ""hora"": {""type"": ""string"", ""description"": ""HH:MM""},
                                        ""evento"": {""type"": ""string"", ""description"": ""Descripcion breve del evento""}
                                    },
                                    ""required"": [""fecha"",""hora"",""evento""],
                                    ""additionalProperties"": false
                                } 
                        },
                        ""evidencias"": {""type"": ""array"", ""items"": {
                                    ""type"": ""object"",
                                    ""properties"": {
                                        ""nombre"": {""type"": ""string"", ""description"": ""Nombre de la evidencia, objeto""},
                                        ""descripcion"": {""type"": ""string"", ""description"": ""Una carta manchada de sangre, un cuchillo con huellas dactilares, una foto de la víctima con un mensaje amenazante, un diario con una página arrancada, etc..""},
                                        ""analisis"": {""type"": ""string"", ""description"": ""Un análisis de la evidencia, puede ser una descripción de lo que se encontró, una conclusión de lo que significa, etc..""},
                                        ""tipo"": {""type"": ""string"", ""description"": ""Arma, Documento, Objeto personal, Foto, Video, etc..""},
                                        ""ubicacion"": {""type"": ""string""}
                                    },
                                    ""required"": [""nombre"",""descripcion"",""analisis"",""tipo"",""ubicacion""],
                                    ""additionalProperties"": false
                                }
                        },
                        ""personajes"": {""type"": ""array"", ""items"": {
                                    ""type"": ""object"",
                                    ""properties"": {
                                        ""nombre"": {""type"": ""string"", ""description"": ""Nombre del personaje""},
                                        ""rol"": {""type"": ""string"", ""description"": ""Testigo, Victima, Cómplice, Informante, Periodista, Familia del sospechoso, etc..""},
                                        ""estado"": {""type"": ""string"", ""description"": ""Vivo, Muerto o Desaparecido""},
                                        ""descripcion"": {""type"":""string"", ""description"": ""Ejemplo: Un hombre mayor con un aire autoritario y una cicatriz prominente en la mejilla. Una mujer joven con gafas grandes y un nerviosismo evidente al hablar. Una ancina amable pero con un comportamiento claramente evasivo. Una joven madre que abraza una foto familiar mientras habla contigo, etc..""},
                                        ""estado_emocional"": {""type"": ""string"", ""description"": ""Nervioso,Tranquilo,Confiado,Arrogante,Asustado,Confuso,Defensivo,Culpable,etc..""},
                                        ""sexo"": {""type"": ""string"", ""description"": ""Masculino o Femenino""}
                                    },
                                    ""required"": [""nombre"",""rol"",""estado"",""descripcion"",""estado_emocional"", ""sexo""],
                                    ""additionalProperties"": false
                                }
                        },
                        ""explicacionCasoResuelto"": {""type"": ""string"", ""description"": ""Descripcion de como se podría resolver el caso""}
                    },
                    ""required"": [""tituloCaso"",""descripcionCaso"",""dificultad"",""fechaOcurrido"",""lugar"",""tiempoRestante"",""cronologia"",""evidencias"",""personajes"",""explicacionCasoResuelto""],
                    ""additionalProperties"": false
                }
            },
            ""required"": [""datosJugador"",""Caso""],
            ""additionalProperties"": false
        }";

    public const string CONVERSATIONS = @"
        {
            ""type"": ""object"",
            ""properties"": {
                ""seHaTerminado"": {
                ""type"": ""boolean"",
                ""description"": ""Indica si la interrogación se considera lógicamente concluida según los criterios definidos (confesión, coartada sólida, estancamiento final, etc.).""
                },
                ""haGanadoUsuario"": {
                ""type"": ""boolean"",
                ""description"": ""Indica si el jugador ha resuelto con éxito el caso respecto a este sospechoso (descubierto la verdad: culpable o inocente). Solo es significativo si seHaTerminado es true.""
                },
                ""razonamiento"": {
                ""type"": ""string"",
                ""description"": ""Explicación concisa (1-2 frases) del motivo de las decisiones tomadas para seHaTerminado y haGanadoUsuario.""
                }
            },
            ""required"": [""seHaTerminado"", ""haGanadoUsuario"", ""razonamiento""],
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