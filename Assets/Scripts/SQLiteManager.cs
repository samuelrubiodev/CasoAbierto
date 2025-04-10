using System.Collections;
using System.Collections.Generic;
using SQLite;
using System.IO;
using UnityEngine;
using System;
using System.Threading.Tasks;

/**
 * Clase que se encarga de la gestión de la base de datos SQLite
 * 
 */
public class SQLiteManager
{

    private string path = string.Empty;
    private SQLiteConnection connection = null;

    private static SQLiteManager instance;

    public SQLiteManager()
    {
        this.path = Application.persistentDataPath + "/database.db";
    }

    public static SQLiteManager GetSQLiteManager()
    {
        if (instance == null)
        {
            instance = new SQLiteManager();
            instance.crearConexion();
        }
        return instance;
    }

    /**
     * Constructor de la clase
     * 
     * @param path: Ruta donde se crear� la base de datos
     */
    public SQLiteManager(string path)
    {
        this.path = path;
    }

    /**
     * Crea la conexión a la base de datos y crea la base de datos si no existe
     * 
     */
    public void crearConexion()
    {
        connection = new SQLiteConnection(path);
    }

    /**
     * Cierra la conexión a la base de datos
     * 
     */
    public void cerrarConexion()
    {
        connection.Close();
    }

    /**
     * Elimina la base de datos, elimina el archivo de la ruta especificada en el constructor
     * 
     */
    public void DeleteDatabase()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Base de datos eliminada");
        }
    }

    /**
     * Crea una tabla en la base de datos
     * 
     * Se debe especificar el tipo de objeto que se va a guardar en la tabla
     */
    public void CreateTable<T>()
    {
        connection.CreateTable<T>();
    }

    /**
     * Sirve solo para ejecutar queries que no devuelven nada, es decir que solo son de INSERT, UPDATE y DELETE
     * 
     * @param query: Query a ejecutar
     */
    public void ExecuteQuery(string query)
    {
        connection.Execute(query);
    }

    /**
     * Sirve para ejecutar queries que devuelven un valor, como SELECT COUNT(*)
     * 
     * @param query: Query a ejecutar
     * @return List<T>: Lista de objetos que devuelve la query
     */
    public List<T> GetTable<T>(string query) where T : new()
    {   
        return connection.Query<T>(query);
    }

    public bool ExistsTable(string tabla)
    {
        string sqlStatement = @"SELECT COUNT(*) FROM " + tabla;
        SQLiteCommand command = connection.CreateCommand(sqlStatement);

        try
        {
            command.ExecuteScalar<int>();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /**
     * Inserta un objeto en la base de datos
     * 
     * @param obj: Objeto a insertar en la base de datos
     */
    public void Insert<T>(T obj)
    {
        connection.Insert(obj);
    }

    public void update<T>(T obj)
    {
        connection.Update(obj);
    }

    public ApiKey[] GetAPIS()
    {
        List<ApiKey> apiKeys = GetTable<ApiKey>("SELECT * FROM ApiKeys");
        ApiKey[] apis = new ApiKey[apiKeys.Count];
        ApiKeyMapper mapper = new ();

        foreach (ApiKey apiKey in apiKeys)
        {
            int index = mapper.GetIndex(apiKey.name);
            if (index >= 0)
            {
                apis[index] = apiKey;
            }
        }
        return apis;
    }

    public Server[] GetServers()
    {
        List<Server> servers = GetTable<Server>("SELECT * FROM Server");
        Server[] serversArray = new Server[servers.Count];
        ServerMapper mapper = new ();

        foreach (Server server in servers)
        {
            int index = mapper.GetIndex(server.nombreServicio);
            if (index >= 0)
            {
                serversArray[index] = server;
            }
        }
        return serversArray;
    }
}
