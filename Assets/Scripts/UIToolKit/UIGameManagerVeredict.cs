using UnityEngine;
using UnityEngine.UIElements;

public class UIGameManagerVeredict : MonoBehaviour
{
    private UIDocument root;
    private static int time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        root = GetComponent<UIDocument>();

        VisualElement buttonsContainer = root.rootVisualElement.Q<VisualElement>("buttons");

        buttonsContainer.Q<Button>("goto-menu").RegisterCallback<ClickEvent>(ev => {
            FinalSceneManager.isUserWin = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        });

        if (FinalSceneManager.isUserWin)
        {
            Label details = root.rootVisualElement.Q<Label>("details");
            details.text = @$"Detalles: 
            - Sospechoso: {MenuPersonajes.personajeSeleccionado.nombre}
            - Veredicto: Culpable
            - Tiempo restante {CoundownTimer.countdownInternal}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
