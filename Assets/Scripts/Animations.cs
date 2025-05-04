using System.Collections;
using TMPro;
using UnityEngine;

public class Animations {

    public static IEnumerator FadeOutCR(TMP_Text text)
    {
        float fadeInTime = 1f;
        float fadeOutTime = 1f;

        while (true)
        {
            float currentTime = 0f;
            while (currentTime < fadeInTime)
            {
                float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInTime);
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
                currentTime += Time.deltaTime;
                yield return null;
            }

            currentTime = 0f;
            while (currentTime < fadeOutTime)
            {
                float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeOutTime);
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
                currentTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}