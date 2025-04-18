using UnityEngine;

public class FootSteps : MonoBehaviour
{

    public AudioSource footsteps;

    void Start()
    {
        Debug.Log("Jugador: " + Jugador.jugador.ToString());
        Debug.Log("Caso Nº1: " + Caso.caso.tituloCaso); 
        Debug.Log("Personaje seleccionado: " + Caso.caso.personajes[0].nombre);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (!GetComponent<FirstPersonController>().playerCanMove) 
            {
                footsteps.enabled = false;
            } else
            {
                footsteps.enabled = true;
            }
        }
        else 
        { 
            footsteps.enabled = false;
        }
    }
}
