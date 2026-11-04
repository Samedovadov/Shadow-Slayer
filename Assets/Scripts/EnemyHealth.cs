
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;

    [Header("Audio")]
    public AudioClip hitSound;

    private int currentHealth;
    private SpriteRenderer sr;
    private Animator animator;
    private AudioSource audioSource;

    void Awake()
    {
        currentHealth = maxHealth;

        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Skeleton HP: " + currentHealth);

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        sr.color = Color.red;

        if (currentHealth <= 0)
        {
            Destroy(gameObject, 0.2f);
        }
        else
        {
            StartCoroutine(HitEffect());
        }
    }

    IEnumerator HitEffect()
    {
        yield return new WaitForSeconds(0.2f);
        sr.color = Color.white;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            PlayerHealth player =
                collision.gameObject.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
}
