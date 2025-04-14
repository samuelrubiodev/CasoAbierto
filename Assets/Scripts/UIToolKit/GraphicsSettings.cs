using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Extensions;

public class GraphicsSettings : MonoBehaviour
{
    public UIDocument uiMainSettings;
    private UIDocument uIDocument;

    void OnEnable()
    {
        uIDocument = GetComponent<UIDocument>();
        VisualElement container = uIDocument.rootVisualElement.Q<VisualElement>("container");
        GraphicsConfig graphicsConfig = new (container.Q<VisualElement>("quality-graphics"));

        uIDocument.rootVisualElement.Q<VisualElement>("buttons").Q<Button>("apply-graphics").RegisterCallback<ClickEvent>(evt =>
        {
            graphicsConfig.ApplyQualitySettings();
        });


        uIDocument.rootVisualElement.Q<VisualElement>("buttons").Q<Button>("back-button").RegisterCallback<ClickEvent>(evt =>
        {
            this.SetActive(false);
            uiMainSettings.SetActive(true);
        });

    }
}
