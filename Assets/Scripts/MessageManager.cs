using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using Utilities.Extensions;

public class UIMessageManager 
{
    private TMP_Text _text;

    public UIMessageManager(TMP_Text text)
    {
        _text = text;
    }

    public IEnumerator ShowMessage(string message)
    {
        _text.text = message;
        _text.gameObject.SetActive(true);

        string[] words = message.Split(' ');
        int chunkSize = 5;

        for (int i = 0; i < words.Length; i += chunkSize)
        {
            string chunk = string.Join(" ", words.Skip(i).Take(chunkSize));
            _text.text = chunk;
            
            float delay = Mathf.Clamp(chunk.Length * 0.1f, 2f, 5f);
            yield return new WaitForSeconds(delay);
        }

        _text.SetActive(false);
    }

    public string GetMessage(string[] strings) {
        string message = string.Join(" ", strings);
        return message;
    }

}