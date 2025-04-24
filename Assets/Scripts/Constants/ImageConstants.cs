using System.Collections.Generic;
using UnityEngine;

public class ImageConstants {

    private static string DataPath = Application.dataPath + "/UI Toolkit/SelectionCharacters/characters";

    public static string DEFAULT_MALE_IMAGE = DataPath + "Mujer2.png";
    public static string DEFAULT_FEMALE_IMAGE = DataPath + "Hombre2.png";

    public static List<string> MALE_IMAGES = new () {
        DataPath + "/Hombre1.png",
        DataPath + "/Hombre2.png",
        DataPath + "/Hombre3.png",
    };
    public static List<string> FEMALE_IMAGES = new () {
        DataPath + "/Mujer1.png",
        DataPath + "/Mujer2.png",
        DataPath + "/Mujer3.png",
    };

}