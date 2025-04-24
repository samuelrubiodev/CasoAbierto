using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.Extensions;

[ExecuteInEditMode]
public class RadialUIController : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 100f;
    public float centerRadius = 50f;
    public float childSize = 40f;
    public float gap = 10f;
    public bool fullCircle = true; 
    [Range(0, 360)]
    public float segmentAngle = 180f; 
    [Range(0, 360)]
    public float rotationOffset = 0f;

    private List<RectTransform> childButtons = new List<RectTransform>();

    public TextMeshProUGUI textEvidence;
    public GameObject resultPanel;

    public SelectionCharacters menuPersonajes;
    public bool Evidencias = false;
    public FirstPersonController FirstPersonController;
    public GameObject evidencePrefab;
    public static Evidencia selectedEvidence;
    public APIRequest aPIRequest;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!Evidencias && !menuPersonajes.Personajes && !ControllerGame.estaEscribiendo)
            {
                ShowRadialMenu();
            }
            else
            {
                HideRadialMenu();
            }
        }
    }


    void ShowRadialMenu()
    {
        Evidencias = true;
        FirstPersonController.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        childButtons.Clear();

        for (int i = 0; i < Caso.caso.evidencias.Count; i++)
        {
            GameObject prefabObject = Instantiate(evidencePrefab, transform);
            prefabObject.SetActive(true);
        }

        foreach (Transform child in transform)
        {
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            child.SetActive(true);
            if (rectTransform != null)
            {
                childButtons.Add(rectTransform);
            }
        }

        RectTransform centerRect = GetComponent<RectTransform>();
        if (centerRect != null)
        {
            centerRect.sizeDelta = new Vector2(centerRadius, centerRadius);
        }

        float angleStep = fullCircle ? 360f / childButtons.Count : segmentAngle / (childButtons.Count - 1);
        float startAngle = fullCircle ? 0f : -segmentAngle / 2;

        for (int i = 0; i < Caso.caso.evidencias.Count; i++)
        {
            Evidencia evidencia = Caso.caso.evidencias[i];
            RectTransform button = childButtons[i];
            float angle = startAngle + i * angleStep + rotationOffset;

            float angleRad = angle * Mathf.Deg2Rad;

            float posX = Mathf.Cos(angleRad) * (radius + gap);
            float posY = Mathf.Sin(angleRad) * (radius + gap);

            button.anchoredPosition = new Vector2(posX, posY);
            button.sizeDelta = new Vector2(childSize, childSize);

            EventTrigger trigger = button.GetComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = trigger.triggers.Find(e => e.eventID == EventTriggerType.PointerEnter);
            EventTrigger.Entry pointerClickEntry = trigger.triggers.Find(e => e.eventID == EventTriggerType.PointerClick);

            pointerEnterEntry.callback.RemoveAllListeners(); 
            pointerEnterEntry.callback.AddListener((data) => {
                textEvidence.SetActive(true);
                textEvidence.text = evidencia.nombre;
                selectedEvidence = evidencia;
            });

            pointerClickEntry.callback.AddListener(async (data) => {
                PointerEventData pointerData = (PointerEventData)data;

                if (pointerData.button == PointerEventData.InputButton.Left)
                {
                    this.SetActive(false);
                    resultPanel.SetActive(true);

                    evidencia = await AnalyzeEvidence(evidencia);

                    resultPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = evidencia.nombre;
                    resultPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = evidencia.analisis;
                    selectedEvidence = evidencia;
                }
                else if (pointerData.button == PointerEventData.InputButton.Right)
                {
                    selectedEvidence = null;
                    HideRadialMenu();
                }
            });
        }
    }

    public async Task<Evidencia> AnalyzeEvidence(Evidencia evidence)
    {
        string json = await aPIRequest.AnalizarEvidencia(evidence);
        JObject jobject = JObject.Parse(json);

        evidence.analisis = jobject["evidencia"]?["resultadoAnalisis"].ToString();

        return evidence;
    }

    public void HideRadialMenu()
    {
        FirstPersonController.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        textEvidence.SetActive(false);
        Evidencias = false;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
