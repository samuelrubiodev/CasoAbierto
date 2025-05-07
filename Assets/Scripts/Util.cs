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

    public static void Show()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void Hide()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}