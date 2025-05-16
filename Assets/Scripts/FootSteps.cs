using OpenAI.Chat;
using UnityEngine;

public class FootSteps : MonoBehaviour
{

    public AudioSource footsteps;
    public bool isEnabled = true;

    void Start()
    {
        Debug.Log("Jugador: " + Jugador.jugador.ToString());
        Debug.Log("Caso Nº1: " + Caso.caso.tituloCaso); 
        Debug.Log("Personaje seleccionado: " + Caso.caso.personajes[0].nombre);
    }

    private void Update()
    {
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if (isMoving && isEnabled)
        {
            if (GetComponent<FirstPersonController>() != null && !GetComponent<FirstPersonController>().playerCanMove)
            {
                if (footsteps.enabled) 
                {
                    footsteps.enabled = false;
                }
            }
            else
            {
                if (!footsteps.enabled)
                {
                    footsteps.enabled = true;
                }
            }
        }
        else
        {
            if (footsteps.enabled)
            {
                footsteps.enabled = false;
            }
        }
    }
}
