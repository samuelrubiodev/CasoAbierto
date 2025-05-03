using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InsufficientElevenLabsCharactersException : Exception
{
    public InsufficientElevenLabsCharactersException () { }
    public InsufficientElevenLabsCharactersException (string message) : base(message) { }
    public InsufficientElevenLabsCharactersException (string message, Exception inner) : base(message, inner) { }
}

public class ElevenLabsImpl : ICreditsAPIManager
{
    private static ElevenLabsImpl _instance;

    private double actualCharacterCount = 0.0;
    private double previousRequestCost = 0.0;
    private UIMessageManager messageManager;
    public double ActualCharacterCount
    {
        get { return actualCharacterCount; }
        set { actualCharacterCount = value; }
    }

    private ElevenLabsImpl(TMP_Text text = null)
    {
        if (text != null)
        {
            messageManager = new UIMessageManager(text);
        }
    }

    public static ElevenLabsImpl Instance(TMP_Text text = null)
    {
        if (_instance == null)
        {
            _instance = new ElevenLabsImpl(text);
        }
        return _instance;
    }

    public static ElevenLabsImpl ResetInstance(TMP_Text text = null)
    {
        if (_instance == null) return null;
        _instance = new ElevenLabsImpl(text);
        return _instance;
    }

    public void VerifyCreditsBalance() 
    {
        // 1. Coste medio por respuesta ElevenLabs (caracteres)
        const int monthlyQuota = 10000;
        int usedChars = (int) ActualCharacterCount;
        int remainingChars = monthlyQuota - usedChars;
        float percentUsed = usedChars / (float)monthlyQuota;  

        // 2. Estimación de peticiones posibles
        const int minCharsPerReq = 69;
        const int maxCharsPerReq = 76;
        int avgCharsPerReq = Mathf.RoundToInt((minCharsPerReq + maxCharsPerReq) / 2f);
        int possibleRequests = remainingChars / avgCharsPerReq;

        new AlertMessage(messageManager.MessageText).SetStyle();

        // 3. Umbrales
        if (percentUsed >= 0.9f && percentUsed < 0.98f)
        {
            CoroutineRunner.Instance
                .StartCoroutine(messageManager.ShowMessage(
                    $"Has consumido {(percentUsed * 100):0}% de tu cuota de voz ({usedChars}/{monthlyQuota} chars)."
                ));
        }
        else if (percentUsed >= 0.98f && remainingChars > 0)
        {
            CoroutineRunner.Instance
                .StartCoroutine(messageManager.ShowMessage(
                    $"¡Quedan solo {remainingChars} chars (~{possibleRequests} síntesis)! Voz reducida."
                ));
        }
        else if (remainingChars <= 0)
        {
            CoroutineRunner.Instance
                .StartCoroutine(messageManager.ShowMessage(
                    "Has agotado tu cuota de voz. No se puede sintetizar voz."
                ));
            throw new InsufficientElevenLabsCharactersException(
                "Suscripción ElevenLabs agotada."
            );
        }
    }

    public Task<double> GetCostRequest(JObject jsonResponse)
    {
        JObject data = jsonResponse["subscription"] as JObject;
        if (data != null && data["character_count"] != null)
        {
            string character_count = data["character_count"].ToString();
            previousRequestCost = Double.Parse(character_count);
        }
        return Task.FromResult(previousRequestCost);
    }

    public Task UpdateCreditsBalance(double amount)
    {
        double amountRequest = amount -= ActualCharacterCount;
        UnityEngine.Debug.Log($"Amount Request: {amountRequest}");
        
        ActualCharacterCount += amountRequest;
        return Task.FromResult(ActualCharacterCount);
    }

    public Task<double> GetActualCreditsBalance(JObject jsonResponse)
    {
        JObject data = jsonResponse["subscription"] as JObject;
        if (data != null && data["character_count"] != null)
        {
            string character_count = data["character_count"].ToString();
            ActualCharacterCount = Double.Parse(character_count);
        }
        return Task.FromResult(ActualCharacterCount);
    }
}