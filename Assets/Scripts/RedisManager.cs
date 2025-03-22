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

    private enum CasoEntityType
    {
        PLAYER,
        EVIDENCE,
        TIMELINE,
        UNKNOWN
    }


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

    private Dictionary<string, Caso> GetCases(long idJugador) 
    {
        Dictionary<string, Caso> casosDictionary = new ();
        
        foreach (var key in GetServer().Keys(pattern: $"jugadores:{idJugador}:caso:*"))
        {
            if (GetDB().KeyType(key) != RedisType.Hash) continue;
        
            string fullKey = key.ToString();
            string[] segments = fullKey.Split(':');
            
            if (segments.Length != 4 || segments[0] != "jugadores" || segments[2] != "caso") continue;
            string idCaso = segments[3];

            if (casosDictionary.ContainsKey(idCaso)) continue;

            Caso caso = Caso.CreateEmptyCaso(idCaso);
            casosDictionary[idCaso] = caso;
            HashEntry[] caso_ = GetHash($"jugadores:{idJugador}:caso:{idCaso}");
            Caso.PopulateCasoBasicInfo(caso_, caso);
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

        Util.AddValues(jugadorHash, mapeo);
        return jugador;
    }

    private CasoEntityType GetEntityTypeFromKey(string key)
    {
        if (key.Contains("personajes")) return CasoEntityType.PLAYER;
        if (key.Contains("evidencias")) return CasoEntityType.EVIDENCE;
        if (key.Contains("cronologia")) return CasoEntityType.TIMELINE;
        return CasoEntityType.UNKNOWN;
    }

    private void AddCasoDetails(long idJugador, Caso caso) 
    {
        foreach (var key in GetServer().Keys(pattern: $"jugadores:{idJugador}:caso:{caso.idCaso}:*"))
        {
            var type = GetDB().KeyType(key);
            if (type != RedisType.Hash) continue;
            
            var hashes = GetDB().HashGetAll(key);

            CasoEntityType entityType = GetEntityTypeFromKey(key.ToString());

            switch (entityType)
            {
                case CasoEntityType.PLAYER:
                    Personaje personaje = Personaje.GetPlayer(hashes);
                    caso.personajes.Add(personaje);
                    break;
                case CasoEntityType.EVIDENCE:
                    Evidencia evidencia = Evidencia.GetEvidence(hashes);
                    caso.evidencias.Add(evidencia);
                    break;
                case CasoEntityType.TIMELINE:
                    Cronologia cronologia = Cronologia.GetTimeline(hashes);
                    caso.cronologia.Add(cronologia);
                    break;
            }
        }
    }

    public Jugador GetPlayer(long id)
    {
        Jugador jugador = GetSimpleJugador(id);
        Dictionary<string, Caso> casosDictionary = GetCases(id);

        foreach (var caso in casosDictionary.Values)
        {
            AddCasoDetails(id, caso);
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
