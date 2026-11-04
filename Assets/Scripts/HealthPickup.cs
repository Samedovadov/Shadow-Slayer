
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Touched: " + other.name);

        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();

            if (player != null && player.hp < 3)
            {
                player.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
