using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FadeLoopOutCommand : ICommandEnumerator
{

    private TMP_Text _Text;

    public FadeLoopOutCommand(TMP_Text text)
    {  
        this._Text = text;
    }

    public IEnumerator Execute()
    {
        float fadeInTime = 1f;
        float fadeOutTime = 1f;

        while (true)
        {
            float currentTime = 0f;
            while (currentTime < fadeInTime)
            {
                float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInTime);
                _Text.color = new Color(_Text.color.r, _Text.color.g, _Text.color.b, alpha);
                currentTime += Time.deltaTime;
                yield return null;
            }

            currentTime = 0f;
            while (currentTime < fadeOutTime)
            {
                float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeOutTime);
                _Text.color = new Color(_Text.color.r, _Text.color.g, _Text.color.b, alpha);
                currentTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}