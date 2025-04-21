using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public delegate void CallBackButton();
public class ErrorOverlayUI : MonoBehaviour
{
    private UIDocument root;
    private Label title;
    private Label description;
    public Button tryButton { get; private set; }
    public Button mainButton { get; private set; }
    public AudioClip errorSound;
    public AudioClip bucleAudioSource;
    public AudioMixer audioMixer;
    public AudioSource audioMusicSource;
    private AudioSource audioSourceErrorSound;
    private AudioSource audioSourceBucle;

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

        tryButton?.RegisterCallback<ClickEvent>(e => {
            tryButtonDelegate?.Invoke();
        });

        mainButton?.RegisterCallback<ClickEvent>(e => {
                mainButtonDelegate?.Invoke();
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
        if (audioMusicSource != null)
        {
            audioMusicSource.Stop();
        }

        audioSourceErrorSound = gameObject.AddComponent<AudioSource>();
        audioSourceErrorSound.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Musica")[0];
        audioSourceErrorSound.clip = errorSound;
        audioSourceErrorSound.Play();

        audioSourceBucle = gameObject.AddComponent<AudioSource>();
        audioSourceBucle.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Musica")[0];
        audioSourceBucle.clip = bucleAudioSource;
        audioSourceBucle.loop = true;
        audioSourceBucle.Play();

        root.rootVisualElement.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        this.title.text = title;
        description.text = message;
    }

    public void HideMenu()
    {
        Time.timeScale = 1f;
        root.rootVisualElement.style.display = DisplayStyle.None;
        audioSourceBucle.Stop();
        audioMusicSource.Play();

        Destroy(audioSourceErrorSound);
        Destroy(audioSourceBucle);
    }
}
