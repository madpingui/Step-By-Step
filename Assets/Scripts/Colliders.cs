using UnityEngine;

public class Colliders : MonoBehaviour
{
    public PlayerController player;

    private void OnTriggerEnter(Collider collision)
    {
        player.next = collision.transform;
    }

    private void OnTriggerExit(Collider collision)
    {
        player.next = null;
    }
}
