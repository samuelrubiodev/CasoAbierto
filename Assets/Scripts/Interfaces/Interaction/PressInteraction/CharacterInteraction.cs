using UnityEngine;

public class CharacterInteraction : MonoBehaviour, IInteraction
{
    public GameObject player;
    private bool showsPlayers = false;

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
                GameObject characters = GameObject.Find("Characters");
                if (!showsPlayers)
                {
                    characters.GetComponent<SelectionCharacters>().Show();
                    showsPlayers = true;
                }
                else
                {
                    characters.GetComponent<SelectionCharacters>().Hide();
                    showsPlayers = false;
                }
            }
        }
    }
}