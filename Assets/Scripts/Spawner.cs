using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] paterns;
    public GameObject last;
    public Transform spawnerParent;

    public Transform init;
    public Transform current;

    public static Spawner Instance { set; get; }

    void Start()
    {
        Instance = this;

        for (int i = 0; i < 10; i++)
        {
            if (i == 0)
            {
                current = init;
            }
            else
            {
                current = last.transform.GetChild(0);
            }

            last = Instantiate(paterns[Random.Range(0, paterns.Length)], current.position, current.rotation);
            last.transform.parent = spawnerParent;
        }
    }

    public void Spawn()
    {
        current = last.transform.GetChild(0);
        last = Instantiate(paterns[Random.Range(0, paterns.Length)], current.position, current.rotation);
        last.transform.parent = spawnerParent;
    }
}
