using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Utilities.Extensions;

public class MainMenu : MonoBehaviour
{
    private UIDocument uiDocument;
    public UIDocument uiMainSettings;
    public UIDocument uiGameManager;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        Label label = uiDocument.rootVisualElement.Q<Label>("button-options");
        Label continueGame = uiDocument.rootVisualElement.Q<Label>("button-continue");
        Label buttonNewGame = uiDocument.rootVisualElement.Q<Label>("button-new-game");

        if (!PlayerPrefs.HasKey("jugadorID")) 
        {
            continueGame.visible = false;
        }

        label.RegisterCallback<ClickEvent>(evt => {
            this.SetActive(false);
            uiMainSettings.SetActive(true);
        });

        continueGame.RegisterCallback<ClickEvent>(evt => {
            this.SetActive(false);
            uiGameManager.SetActive(true);
        });

        buttonNewGame.RegisterCallback<ClickEvent>(evt => {
            ControllerCarga.tieneCaso = false;
            SceneManager.LoadScene("PantallaCarga");
        });
    }
}
