using UnityEngine;
using Utilities.Extensions;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausa;
    private bool pausa = false;
    public FirstPersonController player;
    public Camera cameraMenu;
    public GameObject audioMenu;
    public GameObject audioFondo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausa == false)
            {
                menuPausa.SetActive(true);
                pausa = true;
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                player.playerCamera.enabled = false;
                cameraMenu.SetActive(true);

                audioMenu.SetActive(true);
                audioMenu.GetComponent<AudioSource>().Play();

                audioFondo.SetActive(true);
                audioFondo.GetComponent<AudioSource>().Pause();
            }
            else
            {
                reanudar();
            }
        }
    }

    public void reanudar()
    {
        menuPausa.SetActive(false);
        pausa = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.playerCamera.enabled = true;
        cameraMenu.SetActive(false);

        audioMenu.SetActive(true);
        audioMenu.GetComponent<AudioSource>().Stop();

        audioFondo.SetActive(true);
        audioFondo.GetComponent<AudioSource>().Play();
    }
}