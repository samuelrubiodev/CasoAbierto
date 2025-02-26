using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{

    public AudioSource footsteps;

    void Start()
    {
        Debug.Log("Nombre del jugador: " + Jugador.jugador.nombre);
        Debug.Log("Caso Nº1: " + Jugador.jugador.casos[0].tituloCaso);   
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
