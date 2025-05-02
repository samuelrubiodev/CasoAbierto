using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public interface ICreditsAPIManager {
    Task<double> GetActualCreditsBalance(JObject jsonResponse);
    Task<double> GetCostRequest(JObject jsonResponse);
    Task UpdateCreditsBalance(double amount);
}