using System.Collections.Generic;
using UnityEngine;

public class ImageConstants {

    private static string DataPath = "SelectionCharacters/characters";

    public static string DEFAULT_MALE_IMAGE = DataPath + "Mujer2.png";
    public static string DEFAULT_FEMALE_IMAGE = DataPath + "Hombre2.png";

    public static List<string> MALE_IMAGES = new () {
        DataPath + "/Hombre1",
        DataPath + "/Hombre2",
        DataPath + "/Hombre3",
    };
    public static List<string> FEMALE_IMAGES = new () {
        DataPath + "/Mujer1",
        DataPath + "/Mujer2",
        DataPath + "/Mujer3",
    };

}