using UnityEngine;

public class CaseInteraction : MonoBehaviour, IInteraction
{
    public GameObject player;
    private bool showsCase = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteract();
        }
    }

    public void OnInteract()
    {
        if (Physics.Raycast(player.transform.position, player.transform.TransformDirection(Vector3.forward), out RaycastHit hit, 3f))
        {
            if (hit.collider.gameObject == gameObject)
            {
                GameObject mainPresentation = GameObject.Find("MainPresentation");
                if (!showsCase)
                {
                    mainPresentation.GetComponent<MainPresentation>().Show();
                    showsCase = true;
                    GetComponent<BoxCollider>().enabled = false;
                }
                else
                {
                    mainPresentation.GetComponent<MainPresentation>().Hide();
                    showsCase = false;
                    GetComponent<BoxCollider>().enabled = true;
                }
            }
        }
    }
}
