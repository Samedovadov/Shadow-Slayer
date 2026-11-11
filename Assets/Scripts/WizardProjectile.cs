
using UnityEngine;

public class WizardProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public int damage = 1;
    public float lifetime = 5f;

    [Header("Collision")]
    public LayerMask groundLayer;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);

            Debug.Log("Wizard projectile hit player!");

            Destroy(gameObject);
            return;
        }

        if ((groundLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(gameObject);
        }
    }
}
