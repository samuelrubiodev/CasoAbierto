using UnityEngine;
using UnityEngine.UI;

public class MenuEvidencias : MonoBehaviour
{
    public GameObject content;
    public GameObject scrollView;
    public GameObject buttonPrefab;
    public GameObject panel;
    public FirstPersonController FirstPersonController;
    private bool evidencias = false;
    public static Evidencia evidenciaSeleccionada;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!evidencias)
            {
                MostrarEvidencias();
            }
            else
            {
                OcultarEvidencias();
            }
        }
    }

    public void MostrarEvidencias()
    {
        FirstPersonController.enabled = false;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        RectTransform rt = content.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 100 * Jugador.jugador.casos[Jugador.indexCaso].evidencias.Count);

        for (int i = 0; i < Jugador.jugador.casos[Jugador.indexCaso].evidencias.Count; i++)
        {
            int index = i;
            GameObject button = Instantiate(buttonPrefab, content.transform);

            button.name = Jugador.jugador.casos[Jugador.indexCaso].evidencias[i].nombre;
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = Jugador.jugador.casos[Jugador.indexCaso].evidencias[i].nombre;
            button.SetActive(true);

            Evidencia evidencia = Jugador.jugador.casos[Jugador.indexCaso].evidencias[i];

            button.GetComponent<Button>().onClick.AddListener(() => CargarEvidencia(evidencia));
        }
        
        scrollView.SetActive(true);
        evidencias = true;
        panel.SetActive(true);
    }

    public void DeseleccionarEvidencia()
    {
        evidenciaSeleccionada = null;
        OcultarEvidencias();
    }

    public void CargarEvidencia(Evidencia evidencia)
    {
        evidenciaSeleccionada = evidencia;
        OcultarEvidencias();
    }

    public void OcultarEvidencias()
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
        evidencias = false;
        panel.SetActive(false);
    }
}
