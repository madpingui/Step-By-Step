using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        Destroy(collision.gameObject);

        if(collision.tag == "segunda")
        {
            Destroy(collision.transform.parent.gameObject);
        }
    }
}
