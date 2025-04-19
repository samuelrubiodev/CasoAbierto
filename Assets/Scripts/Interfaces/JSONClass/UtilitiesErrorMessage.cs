using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        string json = System.IO.File
            .ReadAllText(path);

        ErrorsMessage errorMessages = JsonConvert.DeserializeObject<ErrorsMessage>(json);
        return errorMessages;
    }
}
