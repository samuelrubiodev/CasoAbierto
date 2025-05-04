using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Extensions;

public class MainSettings : MonoBehaviour
{
    private UIDocument uiDocument;
    public UIDocument uiMainMenu;
    public UIDocument uiAudio;
    public UIDocument uiGraphics;
    public UIDocument uiScreen;
    public PauseMenu pauseMenu;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        VisualElement container = uiDocument.rootVisualElement.Q<VisualElement>("container");
        container.Q<VisualElement>("back-button").Q<Button>("Button").RegisterCallback<ClickEvent>(evt => {
            this.SetActive(false);
            if (pauseMenu != null) pauseMenu.Show();
            uiMainMenu.SetActive(true);
        });

        Dictionary<string, UIDocument> uiDocuments = new()
        {
            { "configuration-audio", uiAudio },
            { "configuration-graphics", uiGraphics },
            { "configuration-screen", uiScreen }
        };

        for (int i = 0; i < container.childCount; i++)
        {
            VisualElement child = container[i];
            if (uiDocuments.ContainsKey(child.name))
            {
                RegisterCallBacks(child, uiDocuments[child.name]);
            }
        } 
    }

    private void RegisterCallBacks(VisualElement container, UIDocument uiDocument)
    {
        Button button = container.Q<Button>();
        button.RegisterCallback<ClickEvent>(evt => {
            this.SetActive(false);
            uiDocument.SetActive(true);
        });
    }
}
