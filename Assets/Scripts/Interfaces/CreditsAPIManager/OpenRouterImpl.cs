using System;
using System.Collections;
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
        if (_instance.ActualCreditsBalance > 0)
        {
            double actualCreditsBalance = _instance.ActualCreditsBalance;
            _instance = new(subtitle)
            {
                ActualCreditsBalance = actualCreditsBalance
            };
        }
        
        return _instance;
    }

    public IEnumerator VerifyCreditsBalance() 
    {
        const double minCostOR = 0.0001409;
        const double maxCostOR = 0.0001441;
        double avgCostOR = (minCostOR + maxCostOR) / 2.0;

        double roughRemaining = ActualCreditsBalance  / avgCostOR;
        int remainingOR = (int)Math.Floor(roughRemaining);

        new AlertMessage(messageManager.MessageText).SetStyle();

        if (remainingOR <= 20 && remainingOR > 5) {
            yield return messageManager.ShowMessage($"Quedan ~{remainingOR} peticiones de IA",false);
        }
        else if (remainingOR <= 5 && remainingOR > 0) {
            yield return messageManager.ShowMessage($"¡Solo ~{remainingOR} peticiones restantes!",false);
        }
        else if (remainingOR <= 0) {
            yield return messageManager.ShowMessage($"¡No quedan peticiones de IA!",false);
            throw new InsufficientOpenRouterCreditsException("No quedan peticiones de IA.");
        }
    }


    public Task<double> GetCostRequest(JObject jsonResponse)
    {
        JObject data = jsonResponse["data"] as JObject;
        double totalCostDouble = data["total_cost"].Value<double>();
        return Task.FromResult(totalCostDouble);
    }

    public Task UpdateCreditsBalance(double amount)
    {
        double cost = ActualCreditsBalance - amount;
        ActualCreditsBalance = cost;
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