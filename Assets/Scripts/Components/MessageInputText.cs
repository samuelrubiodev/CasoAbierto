using System;
using TMPro;
using UnityEngine;
using Utilities.Extensions;

public class MessageInputText : MonoBehaviour 
{
    public bool canShow = false;
    public bool isSelected = false;
    public string prompt = "";
    public event Action<string> OnPromptSubmitted;
    public FirstPersonController firstPersonController;
    private bool isMessageInputTextEnabled = false;
    private GameObject inputContainer;
    
    void Start()
    {
        inputContainer = gameObject.transform.GetChild(0).gameObject;

        if (PlayerPrefs.GetString("microfono") != "Solo texto")
        {
            gameObject.SetActive(false);
            inputContainer.SetActive(false);
        }
    }

    void Update()
    {
        if (canShow && !isMessageInputTextEnabled && Input.GetKeyDown(KeyCode.B))
        {
            Util.MinShow();
            inputContainer.SetActive(true);
            isMessageInputTextEnabled = true;
            firstPersonController.enabled = false;
        }
        else if (canShow && isMessageInputTextEnabled && Input.GetKeyDown(KeyCode.B)) 
        {
            Util.MinHide();
            inputContainer.SetActive(false);
            isMessageInputTextEnabled = false;
            firstPersonController.enabled = true;
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