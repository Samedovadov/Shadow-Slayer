
using UnityEngine;

public class CrystalPickup : MonoBehaviour
{
    public AudioClip pickupSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player =
                other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.CollectCrystal();

                AudioSource audio =
                    other.GetComponent<AudioSource>();

                if (audio != null && pickupSound != null)
                {
                    audio.PlayOneShot(pickupSound);
                }

                Destroy(gameObject);
            }
        }
    }
}
