using SQLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Table("Server")]
public class Server
{
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
