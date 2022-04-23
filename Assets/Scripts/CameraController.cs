using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController player;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.current.position, 3 * Time.deltaTime);
    }
}
