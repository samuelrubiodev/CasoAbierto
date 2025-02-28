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
                password = await vaultTransit.EncryptAsync("api-key-encrypt", sqliteManager.GetServers()[Server.REDIS].password);
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
        if (db != null)
        {
            db.StringSet(key, value);
        }
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


    public Jugador getJugador(long id)
    {
        HashEntry[] jugadorHash = GetHash($"jugadores:{id}");

        Jugador jugador = new Jugador();

        foreach (HashEntry hashEntry in jugadorHash)
        {
            if (hashEntry.Name == "nombre")
            {
                jugador.nombre = hashEntry.Value;
            }
            else if (hashEntry.Name == "estado")
            {
                jugador.estado = hashEntry.Value;
            }
            else if (hashEntry.Name == "progreso")
            {
                jugador.progreso = hashEntry.Value;
            }
        }

        Dictionary<string, Caso> casosDictionary = new Dictionary<string, Caso>();

        foreach (var key in GetServer().Keys(pattern: $"jugadores:{id}:caso:*"))
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
                        Caso caso = new Caso();
                        caso.idCaso = idCaso;
                        caso.personajes = new List<Personaje>();
                        caso.evidencias = new List<Evidencia>();
                        caso.cronologia = new List<Cronologia>();
                        casosDictionary[idCaso] = caso;

                        HashEntry[] caso_ = GetHash($"jugadores:{id}:caso:{idCaso}");

                        foreach (HashEntry hashEntry in caso_)
                        {
                            if (hashEntry.Name == "tituloCaso")
                            {
                                caso.tituloCaso = hashEntry.Value;
                            }
                            else if (hashEntry.Name == "descripcionCaso")
                            {
                                caso.descripcion = hashEntry.Value;
                            }
                            else if (hashEntry.Name == "dificultad")
                            {
                                caso.dificultad = hashEntry.Value;
                            }
                            else if (hashEntry.Name == "fechaOcurrido")
                            {
                                caso.fechaOcurrido = hashEntry.Value;
                            }
                            else if (hashEntry.Name == "lugar")
                            {
                                caso.lugar = hashEntry.Value;
                            }
                            else if (hashEntry.Name == "tiempoRestante")
                            {
                                caso.tiempoRestante = hashEntry.Value;
                            }
                            else if (hashEntry.Name == "explicacionCasoResuelto")
                            {
                                caso.explicacionCasoResuelto = hashEntry.Value;
                            }
                        }
                    }
                }
            }
        }

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
                        Personaje personaje = new Personaje();
                        foreach (var hashEntry in hashes)
                        {
                            if (hashEntry.Name == "nombre")
                            {
                                personaje.nombre = hashEntry.Value;
                            }
                            else if (hashEntry.Name == "estado")
                            {
                                personaje.estado = hashEntry.Value;
                            } else if (hashEntry.Name == "descripcion")
                            {
                                personaje.descripcion = hashEntry.Value;
                            } else if (hashEntry.Name == "estado_emocional")
                            {
                                personaje.estadoEmocional = hashEntry.Value;
                            } else if (hashEntry.Name == "rol")
                            {
                                personaje.rol = hashEntry.Value;
                            }
                        }
                        caso.personajes.Add(personaje);
                    } 
                    else if (key1.ToString().Contains("evidencias"))
                    {
                        Evidencia evidencia = new Evidencia();
                        foreach (var hashEntry in hashes)
                        {
                            if (hashEntry.Name == "nombre")
                            {
                                evidencia.nombre = hashEntry.Value;
                            }
                            else if (hashEntry.Name == "descripcion")
                            {
                                evidencia.descripcion = hashEntry.Value;
                            } else if (hashEntry.Name == "tipo")
                            {
                                evidencia.tipo = hashEntry.Value;
                            } else if (hashEntry.Name == "analisis")
                            {
                                evidencia.analisis = hashEntry.Value;
                            } else if (hashEntry.Name == "ubicacion")
                            {
                                evidencia.ubicacion = hashEntry.Value;
                            }
                        }
                        caso.evidencias.Add(evidencia);
                    } 
                    else if (key1.ToString().Contains("cronologia"))
                    {
                        Cronologia cronologia = new Cronologia();

                        foreach (var hashEntry in hashes)
                        {
                            if (hashEntry.Name == "fecha")
                            {
                                cronologia.fecha = DateTime.Parse(hashEntry.Value);
                            }
                            else if (hashEntry.Name == "evento")
                            {
                                cronologia.evento = hashEntry.Value;
                            } else if (hashEntry.Name == "hora")
                            {
                                cronologia.hora = hashEntry.Value;
                            }
                        }
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
