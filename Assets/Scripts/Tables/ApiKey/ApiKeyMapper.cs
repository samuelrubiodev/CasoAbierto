using System.Collections.Generic;

public class ApiKeyMapper : IApiKeyMapper
{
    private readonly List<string> _apiNames = new() { "OpenRouter", "ElevenLabs", "Groq" };

    public int GetIndex(string apiName)
    {
        return _apiNames.IndexOf(apiName);
    }
}