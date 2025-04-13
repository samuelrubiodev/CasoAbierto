using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Extensions;

public class MainMenu : MonoBehaviour
{
    private UIDocument uiDocument;
    public UIDocument uiMainSettings;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        Label label = uiDocument.rootVisualElement.Q<Label>("button-options");
        label.RegisterCallback<ClickEvent>(evt => {
            this.SetActive(false);
            uiMainSettings.SetActive(true);
        });
    }
}
