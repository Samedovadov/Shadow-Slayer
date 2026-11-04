
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 8;
    public Slider bossHPBar;

    private int currentHealth;
    private Animator animator;
    private Collider2D bossCollider;
    private bool isDead = false;

    public bool IsDead
    {
        get { return isDead; }
    }

    void Awake()
    {
        currentHealth = maxHealth;

        animator = GetComponentInChildren<Animator>();
        bossCollider = GetComponent<Collider2D>();

        if (bossHPBar != null)
        {
            bossHPBar.minValue = 0;
            bossHPBar.maxValue = maxHealth;
            bossHPBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Max(
            0, currentHealth - damage
        );

        if (bossHPBar != null)
        {
            bossHPBar.value = currentHealth;
        }

        Debug.Log("Boss HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Boss defeated!");

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Die");
        }

        if (bossCollider != null)
        {
            bossCollider.enabled = false;
        }

        StartCoroutine(ShowWinScreen());
    }

    IEnumerator ShowWinScreen()
    {
        yield return new WaitForSeconds(1.2f);

        if (Application.CanStreamedLevelBeLoaded("WinScene"))
        {
            Debug.Log("Loading WinScene");
            SceneManager.LoadScene("WinScene");
        }
        else
        {
            Debug.LogError(
                "WinScene is missing from Scene List!"
            );
        }
    }
}
