using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class OpenRouterImpl : ICreditsAPIManager
{
    private static OpenRouterImpl _instance;

    private double actualCreditsBalance = 0.0;
    public double ActualCreditsBalance
    {
        get { return actualCreditsBalance; }
        set { actualCreditsBalance = value; }
    }

    private OpenRouterImpl() {
    }

    public static OpenRouterImpl Instance
    {
        get
        {
            _instance ??= new OpenRouterImpl();
            return _instance;
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
        ActualCreditsBalance -= amount;
        return Task.FromResult(ActualCreditsBalance);
    }
}