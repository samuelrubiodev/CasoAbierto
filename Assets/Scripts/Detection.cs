using UnityEngine;

public class Detection : MonoBehaviour
{
    public MessageInputText messageInputText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        messageInputText.canShow = true;
    }

    void OnTriggerExit(Collider other)
    {
        messageInputText.canShow = false;
    }
}
