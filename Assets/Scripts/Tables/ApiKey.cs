using SQLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Table("ApiKeys")]
public class ApiKey
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int id { get; set; }

    [Unique]
    [Column("service_name")]
    public string name { get; set; }

    [Column("api_key")]
    public string apiKey { get; set; }

    public ApiKey()
    {
    }
}
