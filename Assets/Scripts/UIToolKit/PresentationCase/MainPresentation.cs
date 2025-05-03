using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Extensions;

public class MainPresentation : MonoBehaviour
{
    public UIDocument uiDocument;
    public FirstPersonController FirstPersonController;
    private TimelinePresentation timelinePresentation;

    void Start()
    {
        uiDocument.SetActive(false);
        timelinePresentation = GetComponent<TimelinePresentation>();
    }

    public void Show()
    {
        uiDocument.SetActive(true);
        Time.timeScale = 0;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        FirstPersonController.enabled = false;

        
    
        VisualElement root = uiDocument.rootVisualElement.Q<VisualElement>("main");
        VisualElement data = root.Q<VisualElement>("data");

        data.Q<Label>("case-name").text = Caso.caso.tituloCaso;
        data.Q<Label>("description").text = Caso.caso.descripcion;

        VisualElement infoData = data.Q<VisualElement>("info-data");
        VisualElement basicData = infoData.Q<VisualElement>("basic-data");
        VisualElement dateTime = infoData.Q<VisualElement>("date-time");
        VisualElement buttons = root.Q<VisualElement>("buttons");

        basicData.Q<Label>("location").text += Caso.caso.lugar;
        basicData.Q<Label>("difficulty").text += Caso.caso.dificultad;

        dateTime.Q<Label>("date").text += Caso.caso.fechaOcurrido.ToString();
        dateTime.Q<Label>("time-remaining").text += Caso.caso.tiempoRestante + " min";

        buttons.Q<Button>("exit-button").RegisterCallback<ClickEvent>(e =>
        {
            GameObject.Find("CaseFile").GetComponent<BoxCollider>().enabled = true;
            Hide();
        });

        buttons.Q<Button>("continue-button").RegisterCallback<ClickEvent>(e =>
        {
            uiDocument.SetActive(false);
            timelinePresentation.Show();
        });
    }

    public void Hide()
    {
        uiDocument.SetActive(false);
        Time.timeScale = 1;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        FirstPersonController.enabled = true; 
    }
}