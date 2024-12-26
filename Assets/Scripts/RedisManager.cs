using System;
using System.IO;
using StackExchange.Redis;
using UnityEngine;
using System.Data;
using SQLite;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Windows;

public class RedisManager
{
    private ConnectionMultiplexer redis;
    private IDatabase db;

    private string ipServer;
    private string port;
    private string user;
    private string password;

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
            Debug.Log($"Nuevo ID generado: {newId}");
            return newId;
        }
        return -1; // Error
    }

    public void SetKey(string key, string value)
    {
        if (db != null)
        {
            db.StringSet(key, value);
            Debug.Log($"Clave '{key}' establecida con valor '{value}'");
        }
    }

    public void SetHash(string key, HashEntry[] hashEntries)
    {
        if (db != null)
        {
            db.StringIncrement("next_id");
            db.HashSet(key, hashEntries);
            Debug.Log($"Hash '{key}' establecido con valores.");
        }
    }

    public string GetKey(string key)
    {
        if (db != null)
        {
            string value = db.StringGet(key);
            Debug.Log($"Clave '{key}' obtenida con valor '{value}'");
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

            Debug.Log($"Hash '{key}' obtenido con valores.");
            return hashEntries;
        }

        return null;
    }


    public void DeleteHash(string key, string field)
    {
        if (db != null)
        { 
            db.HashDelete(key, field);
            Debug.Log($"Campo '{field} de la clave '{key}' borrada con exito'");
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
