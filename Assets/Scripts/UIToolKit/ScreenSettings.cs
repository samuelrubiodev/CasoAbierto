using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Extensions;

public class ScreenSettings : MonoBehaviour
{
    public UIDocument uiMainSettings;
    private UIDocument uIDocument;


    void OnEnable()
    {
        uIDocument = GetComponent<UIDocument>();
        VisualElement container = uIDocument.rootVisualElement.Q<VisualElement>("container");
        Button apply = uIDocument.rootVisualElement.Q<Button>("apply-screen");
        Button back = uIDocument.rootVisualElement.Q<Button>("back-button");

        ModeScreen modeScreen = new (container.Q<VisualElement>("mode-screen"));
        ScreenConfig screenConfig = new (container.Q<VisualElement>("resolution"), uiDocument: uIDocument, modeScreen: modeScreen);
        FPSConfig fpsConfig = new (container.Q<VisualElement>("limit-fps"));

        apply.RegisterCallback<ClickEvent>(evt =>
        {
            screenConfig.ApplyScreenSettings();
            fpsConfig.ApplyFPSSetting();
            modeScreen.ApplyModeScreenSettings();
        });

        back.RegisterCallback<ClickEvent>(evt =>
        {
            this.SetActive(false);
            uiMainSettings.SetActive(true);
        });
    }
}
