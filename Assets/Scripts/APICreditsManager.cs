using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class APICreditsManager : MonoBehaviour
{
    public AudioSource audioSource;
    public bool isGameStarted {get; set;}
    public static JObject jsonOpenRouterResponse;

    private void Start()
    {
        isGameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted) 
        {
            if (audioSource != null && !audioSource.isPlaying && UIMessageManager.isProcessed)
            {
                UpdateElevenLabsCredits();
                UpdateOpenRouterCredits();
                isGameStarted = false;
                UIMessageManager.isProcessed = false;
            }
        }
    }

    private async void UpdateElevenLabsCredits()
    {
        ElevenLabsHttpRequest elevenLabsHttpRequest = new ();
        JObject jsonResponse = await elevenLabsHttpRequest.GetAsync("user");
        
        ElevenLabsImpl elevenLabsImpl = ElevenLabsImpl.Instance();
        await elevenLabsImpl.UpdateCreditsBalance(await elevenLabsImpl.GetCostRequest(jsonResponse));

        Debug.Log("Caracteres restantes: " + elevenLabsImpl.ActualCharacterCount);
    }

    private async void UpdateOpenRouterCredits()
    {
        OpenRouterImpl openRouterImpl = OpenRouterImpl.Instance();
        await openRouterImpl.UpdateCreditsBalance(await openRouterImpl.GetCostRequest(jsonOpenRouterResponse));

        Debug.Log("Creditos restantes: " + openRouterImpl.ActualCreditsBalance);
    }
}
