using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.Extensions;

public class MenuPersonajes : MonoBehaviour
{
    public GameObject content;
    public GameObject scrollView;
    public GameObject personajePrefab;
    public GameObject panel;
    public GameObject panelIzquierda;
    public FirstPersonController FirstPersonController;
    public static Personaje personajeSeleccionado;

    public MenuEvidencias menuEvidencias;
    public bool Personajes = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!Personajes && !menuEvidencias.Evidencias && !ControllerGame.estaEscribiendo)
            {
                MostrarPersonajes();
            }
            else
            {
                OcultarPersonajes();
            }
        }
    }

    public void MostrarPersonajes()
    {
        FirstPersonController.enabled = false;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        RectTransform rt = content.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 100 * Jugador.jugador.casos[Jugador.indexCaso].personajes.Count);

        for (int i = 0; i < Jugador.jugador.casos[Jugador.indexCaso].personajes.Count; i++)
        {
            Personaje personaje = Jugador.jugador.casos[Jugador.indexCaso].personajes[i];

            if (personaje.estado == "Vivo" || personaje.estado == "Viva")
            {
                GameObject objetoPrefab = Instantiate(personajePrefab, content.transform);

                objetoPrefab.name = personaje.nombre;

                objetoPrefab.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = Jugador.jugador.casos[Jugador.indexCaso].personajes[i].nombre;
                objetoPrefab.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = Jugador.jugador.casos[Jugador.indexCaso].personajes[i].estado;
                objetoPrefab.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = Jugador.jugador.casos[Jugador.indexCaso].personajes[i].estadoEmocional;
                objetoPrefab.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = Jugador.jugador.casos[Jugador.indexCaso].personajes[i].rol;

                objetoPrefab.transform.GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) => CargarPersonaje(personaje));
                
                objetoPrefab.SetActive(true);
            }
        }

        scrollView.SetActive(true);
        Personajes = true;
        panel.SetActive(true);
        panelIzquierda.SetActive(true);

        GameObject datosPersonaje = panelIzquierda.transform.GetChild(1).gameObject;

        datosPersonaje.transform.GetChild(0).SetActive(true);
        datosPersonaje.transform.GetChild(1).SetActive(false);
        datosPersonaje.transform.GetChild(2).SetActive(false);
        datosPersonaje.transform.GetChild(3).SetActive(false);
        datosPersonaje.transform.GetChild(4).SetActive(false);
        datosPersonaje.transform.GetChild(5).SetActive(false);
    }

    public void DeseleccionarPersonaje()
    {
        personajeSeleccionado = null;
        OcultarPersonajes();
    }

    public void CargarPersonaje(Personaje personaje)
    {
        personajeSeleccionado = personaje;
        GameObject datosPersonaje = panelIzquierda.transform.GetChild(1).gameObject;
    
        datosPersonaje.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = personaje.nombre;
        datosPersonaje.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = personaje.descripcion;
        datosPersonaje.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = personaje.estado;
        datosPersonaje.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = personaje.estadoEmocional;
        datosPersonaje.transform.GetChild(4).GetComponent<TMPro.TextMeshProUGUI>().text = personaje.rol;

        datosPersonaje.transform.GetChild(0).SetActive(true);
        datosPersonaje.transform.GetChild(1).SetActive(true);
        datosPersonaje.transform.GetChild(2).SetActive(true);
        datosPersonaje.transform.GetChild(3).SetActive(true);
        datosPersonaje.transform.GetChild(4).SetActive(true);
        datosPersonaje.transform.GetChild(5).SetActive(true);
    }

    public void SeleccionarPersonaje()
    {
        OcultarPersonajes();
    }

    public void OcultarPersonajes()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        FirstPersonController.enabled = true;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        scrollView.SetActive(false);
        Personajes = false;
        panel.SetActive(false);
        panelIzquierda.SetActive(false);
    }
}
