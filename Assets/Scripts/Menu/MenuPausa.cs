using UnityEngine;
using Utilities.Extensions;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausa;
    private bool pausa = false;
    public GameObject player;
    public Camera cameraMenu;
    public GameObject audioMenu;
    public GameObject audioFondo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        menuPausa.SetActive(false);
        cameraMenu.SetActive(false);
        audioMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pausa)
            {
                mostrarMenu();
            }
            else
            {
                reanudar();
            }
        }
    }

    public void mostrarMenu()
    {
        menuPausa.SetActive(true);
        pausa = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        cameraMenu.SetActive(true);

        audioMenu.SetActive(true);
        audioMenu.GetComponent<AudioSource>().Play();

        audioFondo.SetActive(true);
        audioFondo.GetComponent<AudioSource>().Pause();
    }

    public void reanudar()
    {
        menuPausa.SetActive(false);
        pausa = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraMenu.SetActive(false);

        audioMenu.SetActive(true);
        audioMenu.GetComponent<AudioSource>().Stop();

        audioFondo.SetActive(true);
        audioFondo.GetComponent<AudioSource>().Play();
    }
}