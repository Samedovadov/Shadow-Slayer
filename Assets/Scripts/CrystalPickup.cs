
using UnityEngine;

public class CrystalPickup : MonoBehaviour
{
    [Header("Item Data")]
    public ItemData itemData;

    [Header("Audio")]
    public AudioClip pickupSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (itemData == null)
        {
            Debug.LogError("CrystalPickup: ItemData is missing!", this);
            return;
        }

        if (itemData.itemType != ItemType.Crystal)
        {
            Debug.LogError("CrystalPickup: Wrong ItemData type!", this);
            return;
        }

        PlayerMovement player =
            other.GetComponent<PlayerMovement>();

        if (player == null)
            return;

        for (int i = 0; i < itemData.amount; i++)
        {
            player.CollectCrystal();
        }

        AudioSource audio =
            other.GetComponent<AudioSource>();

        if (audio != null && pickupSound != null)
        {
            audio.PlayOneShot(pickupSound);
        }

        Destroy(gameObject);
    }
}
