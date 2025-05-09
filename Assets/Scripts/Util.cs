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

    public static void ShowAll()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void HideAll()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void MinShow()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void MinHide()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}