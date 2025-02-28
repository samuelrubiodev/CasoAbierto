using SQLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Table("Server")]
public class Server
{
    public static string IP_SERVER_REDIS = "";
    public static string PORT_SERVER_REDIS = "6379";
    public static string CONTRASENA_REDIS = "";
    public static string NOMBRE_USUARIO_REDIS = ""; 

    public const int REDIS = 0;

    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int id { get; set; }

    [Column("nombre_servicio")]
    [Unique]
    public string nombreServicio { get; set; }

    [Column("ip_server")]
    public string ipServer { get; set; }
    
    [Column("port_server")]
    public int portServer { get; set; }

    [Column("nombre_usuario")]
    public string nombreUsuario { get; set; }

    [Column("password")]
    public string password { get; set; }

    public Server()
    {
    }

    public Server(string nombreServicio, string ipServer, int portServer, string nombreUsuario, string password)
    {
        this.nombreServicio = nombreServicio;
        this.ipServer = ipServer;
        this.portServer = portServer;
        this.nombreUsuario = nombreUsuario;
        this.password = password;
    }
}
