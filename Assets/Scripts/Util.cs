using TMPro;
using UnityEngine;
using Utilities.Extensions;

public static class Util
{
    public static void LoadText(GameObject gameObject, string[] text) {
        for (int i = 0; i < text.Length; i++)
        {
            if (gameObject.transform.GetChild(i).GetComponent<TextMeshProUGUI>() != null)
            {
                gameObject.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = text[i];
            }
        }
    }

    public static void LoadBool(GameObject gameObject, bool[] values) 
    {
        for (int i = 0; i < values.Length; i++)
        {
            gameObject.transform.GetChild(i).SetActive(values[i]);
        }
    }
}