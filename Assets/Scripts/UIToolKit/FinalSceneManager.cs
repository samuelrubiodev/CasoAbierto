using UnityEngine;

public class FinalSceneManager : MonoBehaviour
{
    public static bool isUserWin = false;
    public GameObject screenUserWin;
    public GameObject screenUserLose;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isUserWin)
        {
            screenUserWin.SetActive(true);
            screenUserLose.SetActive(false);
        }
        else
        {
            screenUserWin.SetActive(false);
            screenUserLose.SetActive(true);
        }
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
