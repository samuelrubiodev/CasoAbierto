using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Utilities.Extensions;

public class PauseMenu : MonoBehaviour
{
    public UIDocument uiDocument;
    public UIDocument uiMainSettings;
    public FirstPersonController FirstPersonController;
    public bool pauseMenu = false;

    void Start()
    {
        uiDocument.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           if (!pauseMenu)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
    
    public void Show()
    {
        uiDocument.SetActive(true);
        FirstPersonController.enabled = false;
        FirstPersonController.GetComponent<FootSteps>().enabled = false;
        FirstPersonController.crosshairObject.SetActive(false);
        Time.timeScale = 0;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        VisualElement menuContainer = uiDocument.rootVisualElement.Q<VisualElement>("menu-content-container");
        VisualElement buttonsPanel = menuContainer.Q<VisualElement>("buttons-panel");

        Label label = buttonsPanel.Q<Label>("button-options");

        Label continueGame = buttonsPanel.Q<Label>("button-continue");
        Label buttonNewGame = buttonsPanel.Q<Label>("button-new-game");

        buttonsPanel.Remove(continueGame);
        buttonsPanel.Remove(buttonNewGame);

        Label back = new ("Volver");
        Label exit = new ("Salir");

        back.AddToClassList("menu-button");
        exit.AddToClassList("menu-button");

        back.RegisterCallback<ClickEvent>(evt => {
            Hide();
        });

        exit.RegisterCallback<ClickEvent>(async evt => {
            await new LoadSceneCommand("Menu").Execute();
        });

        label.RegisterCallback<ClickEvent>(evt => {
            uiDocument.SetActive(false);
            uiMainSettings.SetActive(true);
        });

        buttonsPanel.Add(back);
        buttonsPanel.Add(exit);
        pauseMenu = true;
    }

    public void Hide()
    {
        uiDocument.SetActive(false);
        Time.timeScale = 1;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        FirstPersonController.enabled = true;
        FirstPersonController.GetComponent<FootSteps>().enabled = true;
        FirstPersonController.crosshairObject.SetActive(true);
        pauseMenu = false;
    }
}