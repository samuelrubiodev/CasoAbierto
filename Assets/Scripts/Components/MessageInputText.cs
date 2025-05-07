using System;
using TMPro;
using UnityEngine;

public class MessageInputText : MonoBehaviour 
{
    public bool canShow = false;
    public bool isSelected = false;
    public string prompt = "";
    public event Action<string> OnPromptSubmitted;
    private bool isMessageInputTextEnabled = false;
    private GameObject inputContainer;
    
    void Awake()
    {
        inputContainer = gameObject.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (canShow && !isMessageInputTextEnabled && Input.GetKeyDown(KeyCode.B))
        {
            Util.Show();
            inputContainer.SetActive(true);
            isMessageInputTextEnabled = true;
        }
        else if (canShow && isMessageInputTextEnabled && Input.GetKeyDown(KeyCode.B)) 
        {
            Util.Hide();
            inputContainer.SetActive(false);
            isMessageInputTextEnabled = false;
        }

        OnPressEnter();
    }

    public void OnSelect()
    {
        isSelected = true;
    }

    public void OnDeselected()
    {
        isSelected = false;
    }

    public void OnPressEnter()
    {
        if (isSelected && 
            Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            prompt = inputContainer.GetComponentInChildren<TMP_InputField>().text;
            OnPromptSubmitted?.Invoke(prompt);
            inputContainer.SetActive(false);
            isMessageInputTextEnabled = false;
        }
    }
}