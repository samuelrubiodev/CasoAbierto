using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool doorOpen = false;
    public float doorOpenAngle = 95f;
    public float doorCloseAngle = 0.0f;
    public float smooth = 3.0f;

    public AudioClip openDoor;
    public AudioClip closeDoor;

    public void ChangeDoorState()
    {
        doorOpen = !doorOpen;
    }

    private void Update()
    {
        if (doorOpen)
        {
            Quaternion targetRotation = Quaternion.Euler(-90f, doorOpenAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(-90f, doorCloseAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TriggerDoor")
        {
            AudioSource.PlayClipAtPoint(openDoor,transform.position,1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TriggerDoor")
        {
            AudioSource.PlayClipAtPoint(closeDoor, transform.position, 1);
        }
    }
}
