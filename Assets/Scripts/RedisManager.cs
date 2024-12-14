using System;
using System.IO;
using StackExchange.Redis;
using UnityEngine;
using System.Data;
using SQLite;
using GroqApiLibrary;
using System.Text;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json;

public class RedisManager : MonoBehaviour
{
    private ConnectionMultiplexer redis;
    private IDatabase db;

    public string ipServer;
    public string port;
    public string password;

    private string redisConnectionString = "";

    void Start()
    {
        try
        {
            redisConnectionString = $"{ipServer}:{port},password={password}";
            redis = ConnectionMultiplexer.Connect(redisConnectionString);
            db = redis.GetDatabase();

            OnApplicationQuit();
            
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al conectar a Redis: {e.Message}");
        }
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


    public void DeleteHash(string key, string field)
    {
        if (db != null)
        { 
            db.HashDelete(key, field);
            Debug.Log($"Campo '{field} de la clave '{key}' borrada con exito'");
        }
    }


    void OnApplicationQuit()
    {
        if (redis != null)
        {
            redis.Close();
            redis.Dispose();
        }
    }
}
