using SQLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Table("Player")]
public class Player
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int id { get; set; }

    [Column("id_player")]
    [Unique]
    public long idPlayer { get; set; }

    public Player()
    {
        
    }

}
