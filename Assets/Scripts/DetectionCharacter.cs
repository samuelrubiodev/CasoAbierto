using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;

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

    private List<AudioClip> audioClips = new ();

    public TMP_Text textoSubtitulos;
    public GameObject inputField;

    void Start()
    {
        audioClips.Add(audio1);
        audioClips.Add(audio2);
        audioClips.Add(audio3);
        audioClips.Add(audio4);
        audioClips.Add(audio5);
        audioClips.Add(audio6);
        audioClips.Add(audio7);
        audioClips.Add(audio8);
    }

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
            SubtitleManager subtitleManager = new (Application.dataPath + "/Sounds/Speechs/JSON_Subtitles/subtitles.json");
            SubtitleList subtitulos = subtitleManager.ReadJSON();
            AudioSource audio = GetComponent<AudioSource>();

            int numeroAleatorio = Random.Range(1, 9);

            audio.clip = audioClips[numeroAleatorio - 1];
            audio.Play();
            ShowSubtitle(numeroAleatorio, subtitulos);

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
