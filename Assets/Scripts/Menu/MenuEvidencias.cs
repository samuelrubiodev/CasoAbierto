using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuEvidencias : MonoBehaviour
{
    public GameObject content;
    public GameObject scrollView;
    public GameObject evidenciaPrefab;
    public GameObject panel;
    public GameObject panelIzquierda;
    public FirstPersonController FirstPersonController;
    private bool evidencias = false;
    public static Evidencia evidenciaSeleccionada;
    public APIRequest aPIRequest;

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
            GameObject objetoPrefab = Instantiate(evidenciaPrefab, content.transform);

            objetoPrefab.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = Jugador.jugador.casos[Jugador.indexCaso].evidencias[i].nombre;
            objetoPrefab.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => CargarEvidencia(Jugador.jugador.casos[Jugador.indexCaso].evidencias[index]));
            objetoPrefab.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => AnalizarEvidencia(Jugador.jugador.casos[Jugador.indexCaso].evidencias[index]));
        
            objetoPrefab.SetActive(true);
        }
        
        scrollView.SetActive(true);
        evidencias = true;
        panel.SetActive(true);
        panelIzquierda.SetActive(true);
    }

    public async void AnalizarEvidencia(Evidencia evidencia)
    {
        string json = await aPIRequest.AnalizarEvidencia(evidencia);
        Debug.Log(json);
        JObject jobject = JObject.Parse(json);

        GameObject datosEvidencia = panelIzquierda.transform.GetChild(1).gameObject;

        datosEvidencia.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = evidencia.nombre;
        datosEvidencia.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = evidencia.descripcion;
        datosEvidencia.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = evidencia.tipo;
        datosEvidencia.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = jobject["evidencia"]?["resultadoAnalisis"].ToString();
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
        panelIzquierda.SetActive(false);
    }
}
