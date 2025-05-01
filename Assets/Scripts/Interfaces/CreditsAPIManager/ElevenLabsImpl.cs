using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

public class ElevenLabsImpl : ICreditsAPIManager
{
    private static ElevenLabsImpl _instance;

    private double actualCharacterCount = 0.0;
    private double previousRequestCost = 0.0;
    public double ActualCharacterCount
    {
        get { return actualCharacterCount; }
        set { actualCharacterCount = value; }
    }

    private ElevenLabsImpl() {
    }

    public static ElevenLabsImpl Instance
    {
        get
        {
            _instance ??= new ElevenLabsImpl();
            return _instance;
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