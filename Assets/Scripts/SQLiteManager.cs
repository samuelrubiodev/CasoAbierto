using System.Collections;
using System.Collections.Generic;
using SQLite;
using System.IO;
using UnityEngine;

public class SQLiteManager
{

    private string path = string.Empty;
    private SQLiteConnection connection = null;


    public SQLiteManager(string path)
    {
        this.path = path;
    }


    public void crearConexion()
    {
        string connectionString = path;
        connection = new SQLiteConnection(path);
    }

    public void cerrarConexion()
    {
        connection.Close();
    }

    public void CreateDatabase()
    {
        if (!File.Exists(path))
        {
            File.Create(path);
            Debug.Log("Base de datos creada");
        }
        else
        {
            Debug.Log("La base de datos ya existe");
        }
    }

    public void DeleteDatabase()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Base de datos eliminada");
        }
    }

    public void crearTabla(string sqlQuery)
    {
        SQLiteCommand command = connection.CreateCommand(sqlQuery);
        command.ExecuteNonQuery();
    }
}
