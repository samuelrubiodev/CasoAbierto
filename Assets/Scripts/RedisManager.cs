using System;
using System.IO;
using StackExchange.Redis;
using UnityEngine;
using System.Data;
using SQLite;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Windows;
using System.Threading.Tasks;
using NUnit.Framework;

public class RedisManager
{
    private ConnectionMultiplexer redis;
    private IDatabase db;
    private string ipServer;
    private string port;
    private string user;
    private string password;
    private static RedisManager instance;


    public RedisManager(string ipServer, string port, string user, string password)
    {
        this.ipServer = ipServer;
        this.port = port;
        this.password = password;
        this.user = user;
    }

    public RedisManager(string ipServer, string port, string password)
    {
        this.ipServer = ipServer;
        this.port = port;
        this.password = password;
        this.user = "";
    }

    public async static Task<RedisManager> GetRedisManager()
    {
        VaultTransit vaultTransit = new ();

        if (instance == null)
        {
            SQLiteManager sqliteManager = SQLiteManager.GetSQLiteManager();

            string ipServer = "";
            string password = ""; 

            await Task.Run(async () =>
            {
                ipServer = await vaultTransit.DecryptAsync("api-key-encrypt", sqliteManager.GetServers()[Server.REDIS].ipServer);
                password = await vaultTransit.DecryptAsync("api-key-encrypt", sqliteManager.GetServers()[Server.REDIS].password);
            });
            
            instance = new RedisManager(ipServer, "6379", password);
            instance.crearConexion();
            return instance;
        }
        return instance;
    }

    public void crearConexion()
    {
        string redisConnectionString = $"{ipServer}:{port},password={password}";
        redis = ConnectionMultiplexer.Connect(redisConnectionString);
        db = redis.GetDatabase();
    }

    public long GetNewId(string baseKey)
    {
        if (db != null)
        {
            long newId = db.StringIncrement($"{baseKey}:id_counter");
            return newId;
        }
        return -1; // Error
    }

    public void SetKey(string key, string value)
    {
        db?.StringSet(key, value);
    }

    public void SetHash(string key, HashEntry[] hashEntries)
    {
        if (db != null)
        {
            db.StringIncrement("next_id");
            db.HashSet(key, hashEntries);
        }
    }

    public string GetKey(string key)
    {
        if (db != null)
        {
            string value = db.StringGet(key);
            return value;
        }

        return null;
    }

    public IServer GetServer()
    {
        if (db != null)
        {
            return redis.GetServer($"{ipServer}:{port}");
        }
        return null;
    }

    public HashEntry[] GetHash(string key)
    {
        if (db != null)
        {
            HashEntry[] hashEntries = db.HashGetAll(key);
            return hashEntries;
        }

        return null;
    }

    private Dictionary<string, Caso> GetCasos(long idJugador) 
    {
        Dictionary<string, Caso> casosDictionary = new ();
        
        foreach (var key in GetServer().Keys(pattern: $"jugadores:{idJugador}:caso:*"))
        {
            var type = GetDB().KeyType(key);

            if (type == RedisType.Hash)
            {
                string fullKey = key.ToString();
                string[] segments = fullKey.Split(':');

                if (segments.Length == 4 && segments[0] == "jugadores" && segments[2] == "caso") {
                    string idCaso = segments[3];

                    if (!casosDictionary.ContainsKey(idCaso)) 
                    {
                        Caso caso = new()
                        {
                            idCaso = idCaso,
                            personajes = new List<Personaje>(),
                            evidencias = new List<Evidencia>(),
                            cronologia = new List<Cronologia>()
                        };
                        casosDictionary[idCaso] = caso;

                        HashEntry[] caso_ = GetHash($"jugadores:{idJugador}:caso:{idCaso}");

                        var mapeo = new Dictionary<string, Action<string>>
                        {
                            { "tituloCaso", valor => caso.tituloCaso = valor },
                            { "descripcionCaso", valor => caso.descripcion = valor },
                            { "dificultad", valor => caso.dificultad = valor },
                            { "fechaOcurrido", valor => caso.fechaOcurrido = valor },
                            { "lugar", valor => caso.lugar = valor },
                            { "tiempoRestante", valor => caso.tiempoRestante = valor },
                            { "explicacionCasoResuelto", valor => caso.explicacionCasoResuelto = valor }
                        };

                        foreach (HashEntry hashEntry in caso_)
                        {
                            if (mapeo.TryGetValue(hashEntry.Name, out var setter))
                            {
                                setter(hashEntry.Value);
                            }
                        }
                    }
                }
            }
        }
        
        return casosDictionary;
    }

    private Jugador GetSimpleJugador(long idJugador) {
        HashEntry[] jugadorHash = GetHash($"jugadores:{idJugador}");

        Jugador jugador = new()
        {
            idJugador = idJugador
        };

        var mapeo = new Dictionary<string, Action<string>>
        {
            { "nombre", valor => jugador.nombre = valor },
            { "estado", valor => jugador.estado = valor },
            { "progreso", valor => jugador.progreso = valor }
        };

        foreach (HashEntry hashEntry in jugadorHash)
        {
            if (mapeo.TryGetValue(hashEntry.Name, out var setter))
            {
                setter(hashEntry.Value);
            }
        }

        return jugador;
    }

    private Personaje GetPersonaje(HashEntry[] personajeHash) 
    {
        Personaje personaje = new();

        var mapeo = new Dictionary<string, Action<string>>
        {
            { "nombre", valor => personaje.nombre = valor },
            { "estado", valor => personaje.estado = valor },
            { "descripcion", valor => personaje.descripcion = valor },
            { "estado_emocional", valor => personaje.estadoEmocional = valor },
            { "rol", valor => personaje.rol = valor }
        };

        foreach (HashEntry hashEntry in personajeHash)
        {
            if (mapeo.TryGetValue(hashEntry.Name, out var setter))
            {
                setter(hashEntry.Value);
            }
        }

        return personaje;
    }

    private Evidencia GetEvidencia(HashEntry[] evidenciaHash) 
    {
        Evidencia evidencia = new();

        var mapeo = new Dictionary<string, Action<string>>
        {
            { "nombre", valor => evidencia.nombre = valor },
            { "descripcion", valor => evidencia.descripcion = valor },
            { "tipo", valor => evidencia.tipo = valor },
            { "analisis", valor => evidencia.analisis = valor },
            { "ubicacion", valor => evidencia.ubicacion = valor }
        };

        foreach (HashEntry hashEntry in evidenciaHash)
        {
            if (mapeo.TryGetValue(hashEntry.Name, out var setter))
            {
                setter(hashEntry.Value);
            }
        }

        return evidencia;
    }

    private Cronologia GetCronologia(HashEntry[] cronologiaHash) 
    {
        Cronologia cronologia = new();

        var mapeo = new Dictionary<string, Action<string>>
        {
            { "fecha", valor => cronologia.fecha = DateTime.Parse(valor) },
            { "evento", valor => cronologia.evento = valor },
            { "hora", valor => cronologia.hora = valor }
        };

        foreach (HashEntry hashEntry in cronologiaHash)
        {
            if (mapeo.TryGetValue(hashEntry.Name, out var setter))
            {
                setter(hashEntry.Value);
            }
        }

        return cronologia;
    }

    public Jugador getJugador(long id)
    {
        Jugador jugador = GetSimpleJugador(id);
        Dictionary<string, Caso> casosDictionary = GetCasos(id);

        foreach (var caso in casosDictionary.Values)
        {
            foreach (var key1 in GetServer().Keys(pattern: $"jugadores:{id}:caso:{caso.idCaso}:*"))
            {
                var type1 = GetDB().KeyType(key1);

                if (type1 == RedisType.Hash)
                {
                    var hashes = GetDB().HashGetAll(key1);
        
                    if (key1.ToString().Contains("personajes"))
                    {
                        Personaje personaje = GetPersonaje(hashes);
                        caso.personajes.Add(personaje);
                    } 
                    else if (key1.ToString().Contains("evidencias"))
                    {
                        Evidencia evidencia = GetEvidencia(hashes);
                        caso.evidencias.Add(evidencia);
                    } 
                    else if (key1.ToString().Contains("cronologia"))
                    {
                        Cronologia cronologia = GetCronologia(hashes);
                        caso.cronologia.Add(cronologia);
                    }
                }
            }
        }

        jugador.casos = new List<Caso>(casosDictionary.Values);
        return jugador;
    }


    public void DeleteHash(string key, string field)
    {
        if (db != null)
        { 
            db.HashDelete(key, field);
        }
    }

    public void cerrarConexion()
    {
        if (redis != null)
        {
            redis.Close();
            redis.Dispose();
        }
    }

    public IDatabase GetDB()
    {
        if (db != null)
        {
            return db;
        }
        return null;
    }
}
