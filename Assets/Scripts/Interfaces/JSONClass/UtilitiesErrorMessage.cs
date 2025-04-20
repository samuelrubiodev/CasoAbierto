using Newtonsoft.Json;

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
