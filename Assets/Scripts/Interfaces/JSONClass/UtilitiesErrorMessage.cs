using Newtonsoft.Json;
using UnityEngine;

public class UtilitiesErrorMessage : IJsonUtilities<ErrorsMessage>
{
    private readonly string path;

    public UtilitiesErrorMessage(string jsonPath)
    {
        this.path = jsonPath;
    }

    public ErrorsMessage ReadJSON()
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        ErrorsMessage errorMessages = JsonConvert.DeserializeObject<ErrorsMessage>(json.text);
        return errorMessages;
    }
}
