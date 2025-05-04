using System.Collections;
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
                ProcessCreditsAsync();
                StartCoroutine(CreditsFlowRoutine());

                isGameStarted = false;
                UIMessageManager.isProcessed = false;
            }
        }
    }

    private IEnumerator CreditsFlowRoutine()
    {
        OpenRouterImpl openRouterImpl = OpenRouterImpl.Instance();
        ElevenLabsImpl elevenLabsImpl = ElevenLabsImpl.Instance();

        yield return openRouterImpl.VerifyCreditsBalance();
        yield return elevenLabsImpl.VerifyCreditsBalance();
    }

    private async void ProcessCreditsAsync()
    {
        await UpdateElevenLabsCredits();
        await UpdateOpenRouterCredits();
    }

    private async Task UpdateElevenLabsCredits()
    {
        ElevenLabsHttpRequest elevenLabsHttpRequest = new ();
        JObject jsonResponse = await elevenLabsHttpRequest.GetAsync("user");
        
        ElevenLabsImpl elevenLabsImpl = ElevenLabsImpl.Instance();
        await elevenLabsImpl.UpdateCreditsBalance(await elevenLabsImpl.GetCostRequest(jsonResponse));

        Debug.Log("Caracteres restantes: " + elevenLabsImpl.ActualCharacterCount);
    }

    private async Task UpdateOpenRouterCredits()
    {
        OpenRouterImpl openRouterImpl = OpenRouterImpl.Instance();
        double cost = await openRouterImpl.GetCostRequest(jsonOpenRouterResponse);
        await openRouterImpl.UpdateCreditsBalance(cost);

        Debug.Log("Creditos restantes: " + openRouterImpl.ActualCreditsBalance);
    }
}
