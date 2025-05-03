using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Extensions;

public class TimelinePresentation : MonoBehaviour
{
    public UIDocument uiDocument;
    private MainPresentation mainPresentation;
    public FirstPersonController FirstPersonController;

    void Start()
    {
        uiDocument.SetActive(false);
        mainPresentation = GetComponent<MainPresentation>();
    }

    public void Show()
    {
        uiDocument.SetActive(true);
        Time.timeScale = 0;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        FirstPersonController.enabled = false;

        VisualElement root = uiDocument.rootVisualElement.Q<VisualElement>("main");
        VisualElement container = root.Q<VisualElement>("container");
        ScrollView scrollView = container.Q<ScrollView>("timelineScroll");
        VisualElement buttons = root.Q<VisualElement>("buttons");

        scrollView.Clear();
        scrollView.AddToClassList("timelineScroll");

        for (int i = 0; i < Caso.caso.cronologia.Count; i++)
        {
            Cronologia cronologia = Caso.caso.cronologia[i];
            VisualElement eventElement = new ();

            eventElement.AddToClassList("container-timeline");

            Label eventHourTime = new ();
            eventHourTime.AddToClassList("mediumTitle");
            eventHourTime.text = cronologia.fecha + " " + cronologia.hora;

            Label eventDescription = new ();
            eventDescription.AddToClassList("small-text");
            eventDescription.text = cronologia.evento;

            eventElement.Add(eventHourTime);
            eventElement.Add(eventDescription);

            scrollView.Add(eventElement);
        }

        buttons.Q<Button>("exit-button").RegisterCallback<ClickEvent>(e =>
        {
            GameObject.Find("CaseFile").GetComponent<BoxCollider>().enabled = true;
            Hide();
        });

        buttons.Q<Button>("back-button").RegisterCallback<ClickEvent>(e =>
        {
            scrollView.Clear();
            scrollView.AddToClassList("timelineScroll");

            uiDocument.SetActive(false);
            mainPresentation.Show();
        });
    }

    public void Hide()
    {
        VisualElement root = uiDocument.rootVisualElement.Q<VisualElement>("main");
        VisualElement container = root.Q<VisualElement>("container");
        ScrollView scrollView = container.Q<ScrollView>("timelineScroll");

        scrollView.Clear();
        scrollView.AddToClassList("timelineScroll");

        uiDocument.SetActive(false);
        Time.timeScale = 1;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        FirstPersonController.enabled = true; 
    }

}