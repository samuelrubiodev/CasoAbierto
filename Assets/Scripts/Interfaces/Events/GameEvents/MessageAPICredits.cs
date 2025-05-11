using Newtonsoft.Json.Linq;

public class MessageAPICredits : GameEvent
{
    public JObject jsonOpenRouterResponse;
    public MessageAPICredits(JObject jsonOpenRouterResponse)
    {
        this.jsonOpenRouterResponse = jsonOpenRouterResponse;
    }
}