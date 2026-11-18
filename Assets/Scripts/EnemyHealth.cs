
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Data")]
    public EnemyData enemyData;

    [Header("Audio")]
    public AudioClip hitSound;

    private int currentHealth;
    private SpriteRenderer sr;
    private Animator animator;
    private AudioSource audioSource;

    private int MaxHealth => enemyData != null
        ? Mathf.Max(1, enemyData.maxHealth)
        : 3;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (enemyData == null)
        {
            SkeletonPatrol skeleton = GetComponent<SkeletonPatrol>();

            if (skeleton != null)
            {
                enemyData = skeleton.enemyData;
            }
        }

        if (enemyData == null)
        {
            Debug.LogError(
                "EnemyHealth: EnemyData is not assigned!",
                this
            );

            enabled = false;
            return;
        }

        currentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (!enabled || damage <= 0)
            return;

        currentHealth -= damage;

        Debug.Log("Skeleton HP: " + currentHealth);

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (sr != null)
        {
            sr.color = Color.red;
        }

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

        if (sr != null)
        {
            sr.color = Color.white;
        }
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
