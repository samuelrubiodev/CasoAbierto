using UnityEngine;
using Utilities.Extensions;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausa;
    private bool pausa = false;
    public Camera cameraMenu;
    public GameObject audioMenu;
    public GameObject audioFondo;
    public MenuEvidencias menuEvidencias;
    public MenuPersonajes menuPersonajes;

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
            if (!pausa && !menuEvidencias.Evidencias && !menuPersonajes.Personajes)
            {
                MostrarMenu();
            }
            else
            {
                Reanudar();
            }
        }
    }

    public void MostrarMenu()
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

    public void Reanudar()
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