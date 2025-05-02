using UnityEngine;

public class SittingChairInteraction : MonoBehaviour, IInteraction
{
    public GameObject player;
    private Vector3 originalPosition;

    private bool isSitting = false;

    void Update()
    {
        OnInteract();
    }

    public void OnInteract()
    {
        if (!isSitting && Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(player.transform.position, player.transform.forward, out RaycastHit hit, 2f))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
                if (hit.collider.gameObject == gameObject)
                {
                    SitDown();
                }
            }
        }
        else if (isSitting && Input.GetKeyDown(KeyCode.F))
        {
            StandUp();
        }
    }

    private void SitDown()
    {
        player.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<MeshCollider>().enabled = false;

        originalPosition = player.transform.position;
        player.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);

        var controller = player.GetComponent<FirstPersonController>();
        controller.playerCanMove = false;
        controller.enableJump = false;

        isSitting = true;
    }

    private void StandUp()
    {
        player.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        player.transform.position = originalPosition;

        var controller = player.GetComponent<FirstPersonController>();
        controller.playerCanMove = true;
        controller.enableJump = true;

        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<MeshCollider>().enabled = true;
        player.transform.rotation = Quaternion.identity;

        isSitting = false;
    }
}
