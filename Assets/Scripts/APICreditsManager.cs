using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class APICreditsManager : MonoBehaviour
{
    public AudioSource audioSource;
    public bool isGameStarted {get; set;}

    private void Start()
    {
        isGameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted) 
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                UpdateCredits();
                isGameStarted = false;
            }
        }
    }

    private async void UpdateCredits()
    {
        ElevenLabsHttpRequest elevenLabsHttpRequest = new ();
        JObject jsonResponse = await elevenLabsHttpRequest.GetAsync("user");
        
        ElevenLabsImpl elevenLabsImpl = ElevenLabsImpl.Instance;
        await elevenLabsImpl.UpdateCreditsBalance(await elevenLabsImpl.GetCostRequest(jsonResponse));

        Debug.Log("Caracteres restantes: " + elevenLabsImpl.ActualCharacterCount);
    }
}
