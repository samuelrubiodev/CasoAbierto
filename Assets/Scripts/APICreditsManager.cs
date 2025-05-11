using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class APICreditsManager : MonoBehaviour
{
    public AudioSource audioSource;
    public bool isGameStarted {get; set;}
    public JObject jsonOpenRouterResponse;

    private void Awake()
    {
        EventManager.GetInstance().Subscribe<MessageAPICredits>(OnArrivedMessage);
    }

    private void OnDestroy()
    {
        EventManager.GetInstance().Unsubscribe<MessageAPICredits>(OnArrivedMessage);
    }
    
    public void OnArrivedMessage(MessageAPICredits message)
    {
        jsonOpenRouterResponse = message.jsonOpenRouterResponse;

        if (audioSource != null && !audioSource.isPlaying)
        {
            ProcessCreditsAsync();
            StartCoroutine(CreditsFlowRoutine());
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
