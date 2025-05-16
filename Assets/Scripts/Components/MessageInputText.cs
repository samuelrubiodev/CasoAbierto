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
    public GameObject player;
    private FirstPersonController firstPersonController;
    private bool isMessageInputTextEnabled = false;
    private GameObject inputContainer;
    
    void Start()
    {
        inputContainer = gameObject.transform.GetChild(0).gameObject;
        firstPersonController = player.GetComponent<FirstPersonController>();

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
            player.GetComponent<FootSteps>().isEnabled = false;
        }
        else if (canShow && isMessageInputTextEnabled && Input.GetKeyDown(KeyCode.B))
        {
            inputContainer.SetActive(false);
            isMessageInputTextEnabled = false; 
            Util.MinHide();
            firstPersonController.enabled = true;
            player.GetComponent<FootSteps>().isEnabled = true;
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
            Util.MinHide();
            
            firstPersonController.enabled = true;
            player.GetComponent<FootSteps>().isEnabled = true;
        }
    }
}