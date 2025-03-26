using System.Collections.Generic;

public class ServerMapper : IServerMapper
{
    private readonly List<string> _apiNames = new() { "Redis"};
    public int GetIndex(string name)
    {
        return _apiNames.IndexOf(name);
    }
}