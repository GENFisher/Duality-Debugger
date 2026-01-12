using UnityEngine;

public class Swap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<PlayerController>().Swap();
    }

}
