using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class LoadSceneCommand : ICommand
{
    private readonly string sceneName;

    public LoadSceneCommand(string sceneName)
    {
        this.sceneName = sceneName;
    }

    public async Task Execute()
    {
        var execute = SceneManager.LoadSceneAsync(sceneName);
        while (!execute.isDone)
        {
            await Task.Yield();
        }
    }
}