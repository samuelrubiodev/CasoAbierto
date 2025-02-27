using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Windows;

public class DetectionCharacter : MonoBehaviour
{

    // Variables de audio
    public AudioClip audio1;
    public AudioClip audio2;
    public AudioClip audio3;
    public AudioClip audio4;
    public AudioClip audio5;
    public AudioClip audio6;
    public AudioClip audio7;
    public AudioClip audio8;
    private AudioSource audioSource;

    private bool isPlaying = false;

    public static void ShowSubtitle(int id, SubtitleList subtitulos)
    {
        bool subtitleFound = false;

        for (int i = 0; i < subtitulos.subtitles.Count; i++)
        {
            Subtitle sub = subtitulos.subtitles[i];

            if (sub.id == id)
            {
                subtitleFound = true;
                foreach (string s in sub.text)
                {
                    Debug.Log("ID: " + sub.id + " Texto: " + s);
                }
                break;
            }
        }

        if (!subtitleFound)
        {
            Debug.LogWarning("No se encontró un subtítulo con el ID: " + id);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlaying)
        {
            SubtitleManager subtitleManager = new SubtitleManager(Application.dataPath + "/Sounds/Speechs/JSON_Subtitles/subtitles.json");

            SubtitleList subtitulos = subtitleManager.ReadJSON();

            AudioSource audio = GetComponent<AudioSource>();

            int numeroAleatorio = 0;

            // Generar un numero aleatorio entre 1 y 8 para reproducir un audio aleatorio
            numeroAleatorio = UnityEngine.Random.Range(1, 9);

            switch (numeroAleatorio)
            {
                case 1:
                    audio.clip = audio1;
                    audio.Play();
                    ShowSubtitle(1, subtitulos);
                    break;
                case 2:
                    audio.clip = audio2;
                    audio.Play();
                    ShowSubtitle(2, subtitulos);
                    break;
                case 3:
                    audio.clip = audio3;
                    audio.Play();
                    ShowSubtitle(3, subtitulos);
                    break;
                case 4:
                    audio.clip = audio4;
                    audio.Play();
                    ShowSubtitle(4, subtitulos);
                    break;
                case 5:
                    audio.clip = audio5;
                    audio.Play();
                    ShowSubtitle(5, subtitulos);
                    break;
                case 6:
                    audio.clip = audio6;
                    audio.Play();
                    ShowSubtitle(6, subtitulos);
                    break;
                case 7:
                    audio.clip = audio7;
                    audio.Play();
                    ShowSubtitle(7, subtitulos);
                    break;
                case 8:
                    audio.clip = audio8;
                    audio.Play();
                    ShowSubtitle(8, subtitulos);
                    break;
            }

            isPlaying = true;
            this.audioSource = audio;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    void Update()
    {
        if (this.audioSource != null && !this.audioSource.isPlaying)
        {
            GameObject controllerGame = GameObject.Find("ControllerGame");
            controllerGame.GetComponent<ControllerGame>().isGameInProgress = true;
        }
    }
}
