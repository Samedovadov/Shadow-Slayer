
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Item Data")]
    public ItemData itemData;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (itemData == null)
        {
            Debug.LogError("HealthPickup: ItemData is missing!", this);
            return;
        }

        if (itemData.itemType != ItemType.Health)
        {
            Debug.LogError("HealthPickup: Wrong ItemData type!", this);
            return;
        }

        PlayerHealth player =
            other.GetComponent<PlayerHealth>();

        if (player == null)
            return;

        if (player.hp < 3)
        {
            player.Heal(itemData.amount);
            Destroy(gameObject);
        }
    }
}
