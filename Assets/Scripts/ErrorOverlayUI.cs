using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Extensions;

public delegate void CallBackButton();
public class ErrorOverlayUI : MonoBehaviour
{
    private UIDocument root;
    private Label title;
    private Label description;
    public Button tryButton { get; private set; }
    public Button mainButton { get; private set; }

    private CallBackButton tryButtonDelegate;
    private CallBackButton mainButtonDelegate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        root = GetComponent<UIDocument>();
        VisualElement header = root.rootVisualElement.Q<VisualElement>("header");
        VisualElement buttons = root.rootVisualElement.Q<VisualElement>("buttons");

        title = header.Q<Label>("title");
        description = root.rootVisualElement.Q<Label>("description");

        tryButton = buttons.Q<Button>("retry-button");
        mainButton = buttons.Q<Button>("goto-menu");

        tryButton.RegisterCallback<ClickEvent>(e => {
            tryButtonDelegate?.Invoke();
        });

        root.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void SetTryCallBackButtonDelegate(CallBackButton tryButtonDelegate)
    {
        this.tryButtonDelegate = tryButtonDelegate;
    }

    public void SetMainMenuCallBackButtonnDelegate(CallBackButton mainButtonDelegate)
    {
        this.mainButtonDelegate = mainButtonDelegate;
    }

    public void ShowError(string title, string message) 
    {
        root.rootVisualElement.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
        this.title.text = title;
        description.text = message;
    }

    public void HideMenu()
    {
        Time.timeScale = 1f;
        root.rootVisualElement.style.display = DisplayStyle.None;
    }
}
