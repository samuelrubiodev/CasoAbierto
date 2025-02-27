using UnityEngine;
using UnityEngine.UIElements;

public class Prueba : MonoBehaviour
{
    public UIDocument uIDocument;
    Button boton;

    void OnEnable()
    {
        uIDocument = GetComponent<UIDocument>();
        boton = uIDocument.rootVisualElement.Q<Button>("BotonJugar");

        boton.RegisterCallback<ClickEvent>(evento => Debug.Log("Boton presionado"));
        
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
