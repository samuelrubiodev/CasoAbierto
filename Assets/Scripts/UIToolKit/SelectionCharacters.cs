using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Extensions;

public class SelectionCharacters : MonoBehaviour
{
    public static Personaje selectedCharacter;
    public RadialUIController radialUIController;
    public FirstPersonController FirstPersonController;
    public UIDocument uiCharacters;
    private List<int> charactersThatHaveGoneOut = new ();
    private List<string> pathsImage = new ();
    public static bool hasChangedCharacter = true;
    public bool Personajes = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectedCharacter = Caso.caso.personajes[0];
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        pathsImage = Personaje.GetRandomImages(Caso.caso.personajes,charactersThatHaveGoneOut);

        uiCharacters.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!Personajes && !radialUIController.Evidencias && !ControllerGame.estaEscribiendo)
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
        Personajes = true;
        uiCharacters.SetActive(true);
        FirstPersonController.enabled = false;
        Time.timeScale = 0;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;

        List<Personaje> characters = Caso.caso.personajes;
        VisualElement charactersElements = uiCharacters.rootVisualElement.Q<VisualElement>("characters");

        for (int i = 0; i < characters.Count; i++)
        {
            Personaje personaje = characters[i];

            VisualElement character = new ();
            character.AddToClassList("mainCharacter");

            byte[] image = File.ReadAllBytes(pathsImage[i]);

            Texture2D imageTexture = new (1,1);
            imageTexture.LoadImage(image);

            VisualElement characterImage = new ();
            characterImage.AddToClassList("character");
            characterImage.style.backgroundImage = imageTexture;

            Label characterName = new ();
            characterName.AddToClassList("characterName");
            characterName.text = personaje.nombre;

            Button buttonCharacterSelect = new ();
            buttonCharacterSelect.AddToClassList("button");
            buttonCharacterSelect.AddToClassList("buttons");
            buttonCharacterSelect.text = "Seleccionar";

            Button buttonAccuse = new ();
            buttonAccuse.AddToClassList("button");
            buttonAccuse.text = "Acusar";

            buttonCharacterSelect.RegisterCallback<ClickEvent>( e => {
                selectedCharacter = personaje;
                hasChangedCharacter = true;
                Hide();
            });

            buttonAccuse.RegisterCallback<ClickEvent>(async e => {
                GameStatus gameStatus = new ();
                JObject jsonGameStatus = await Task.Run(async () => await gameStatus.SendRequest(prompt: "Analiza esta conversacion: \n"));
                new ControllerGameManager().Veredict(jsonGameStatus);
            });

            charactersElements.Add(character);

            character.Add(characterImage);
            character.Add(characterName);
            character.Add(buttonCharacterSelect);
            character.Add(buttonAccuse);
        }


    }

    public void Hide()
    {
        uiCharacters.rootVisualElement.Q<VisualElement>("characters").Clear();
        uiCharacters.SetActive(false);
        Personajes = false;
        Time.timeScale = 1;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        FirstPersonController.enabled = true;
    }
}
