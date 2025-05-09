using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;

public class DetectionCharacter : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private TMP_Text textoSubtitulos;
    private AudioSource audioSource;

    private bool isPlaying = false;

    public async void ShowSubtitle(int id, SubtitleList subtitulos)
    {
        bool subtitleFound = false;

        textoSubtitulos.gameObject.SetActive(true);

        new MessageStyleManager(textoSubtitulos).SetStyle();

        for (int i = 0; i < subtitulos.subtitles.Count; i++)
        {
            Subtitle sub = subtitulos.subtitles[i];
            if (sub.id == id)
            {
                subtitleFound = true;
                foreach (string s in sub.text)
                {
                    textoSubtitulos.text = s;
                    await Task.Delay(2000);
                }
                break;
            }
        }

        textoSubtitulos.gameObject.SetActive(false);

        if (!subtitleFound)
        {
            Debug.LogWarning("No se encontró un subtítulo con el ID: " + id);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlaying)
        {
            SubtitleManager subtitleManager = new ("Speechs/JSON_Subtitles/subtitles");
            SubtitleList subtitulos = subtitleManager.ReadJSON();
            AudioSource audio = GetComponent<AudioSource>();

            int numeroAleatorio = Random.Range(0, audioClips.Length);

            audio.clip = audioClips[numeroAleatorio];
            audio.Play();
            ShowSubtitle(numeroAleatorio, subtitulos);

            isPlaying = true;
            this.audioSource = audio;
        }
    }
    void Update()
    {
        if (this.audioSource != null && !this.audioSource.isPlaying && GameObject.Find("ControllerGame") != null)
        {
            GameObject controllerGame = GameObject.Find("ControllerGame");
            controllerGame.GetComponent<ControllerGame>().isGameInProgress = true;
        }
    }
}
