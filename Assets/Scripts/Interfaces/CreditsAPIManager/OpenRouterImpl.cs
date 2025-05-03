using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InsufficientOpenRouterCreditsException : Exception
{
    public InsufficientOpenRouterCreditsException () { }
    public InsufficientOpenRouterCreditsException (string message) : base(message) { }
    public InsufficientOpenRouterCreditsException (string message, Exception inner) : base(message, inner) { }
}

public class OpenRouterImpl : ICreditsAPIManager
{
    private static OpenRouterImpl _instance;
    private double actualCreditsBalance = 0.0;
    private UIMessageManager messageManager;
    public double ActualCreditsBalance
    {
        get { return actualCreditsBalance; }
        set { actualCreditsBalance = value; }
    }

    private OpenRouterImpl(TMP_Text subtitle = null)
    {
        if (subtitle != null)
        {
            messageManager = new UIMessageManager(subtitle);
        }
    }

    public static OpenRouterImpl Instance(TMP_Text subtitle = null)
    {
        if (_instance == null)
        {
            _instance = new OpenRouterImpl(subtitle);
        }
        return _instance;
    }

    public static OpenRouterImpl ResetInstance(TMP_Text subtitle = null)
    {
        if (_instance == null) return null;
        _instance = new OpenRouterImpl(subtitle);
        return _instance;
    }

    public void VerifyCreditsBalance() 
    {
        // 1. Coste medio por petición OpenRouter
        const float minCostOR = 0.0001409f;
        const float maxCostOR = 0.0001441f;
        float avgCostOR = (minCostOR + maxCostOR) / 2f;

        // 2. Cálculo de peticiones restantes
        int remainingOR = (int)(0.002 / avgCostOR);

        new AlertMessage(messageManager.MessageText).SetStyle();

        // 3. Umbrales de aviso y degradación
        if (remainingOR <= 20 && remainingOR > 5) {
            CoroutineRunner.Instance.StartCoroutine(messageManager.ShowMessage($"Quedan ~{remainingOR} peticiones de IA"));
        }
        else if (remainingOR <= 5 && remainingOR > 0) {
            CoroutineRunner.Instance.StartCoroutine(messageManager.ShowMessage($"¡Solo ~{remainingOR} peticiones restantes! Diálogos reducidos."));
        }
        else if (remainingOR <= 0) {
            CoroutineRunner.Instance.StartCoroutine(messageManager.ShowMessage("No quedan peticiones de IA. Diálogos desactivados."));
            throw new InsufficientOpenRouterCreditsException ("No quedan peticiones de IA. Diálogos desactivados.");
        }
    }

    public Task<double> GetCostRequest(JObject jsonResponse)
    {
        JObject data = jsonResponse["data"] as JObject;
        string totalCost = data["total_cost"].ToString();
        double totalCostDouble = Double.Parse(totalCost);
        return Task.FromResult(totalCostDouble);
    }

    public Task UpdateCreditsBalance(double amount)
    {
        double amountRequest = amount - ActualCreditsBalance;
        ActualCreditsBalance -= amountRequest;
        return Task.FromResult(ActualCreditsBalance);
    }

    public Task<double> GetActualCreditsBalance(JObject jsonResponse)
    {
        JObject data = jsonResponse["data"] as JObject;
        string totalCost = data["total_credits"].ToString();
        string totalUsage = data["total_usage"].ToString();
        double totalCredits = Double.Parse(totalCost);
        double totalUsageDouble = Double.Parse(totalUsage);
        ActualCreditsBalance = totalCredits - totalUsageDouble;
        return Task.FromResult(ActualCreditsBalance);
    }
}