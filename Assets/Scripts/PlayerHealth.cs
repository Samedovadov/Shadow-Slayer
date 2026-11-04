
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int hp = 3;
    public float fallLimit = -8f;
    public TMP_Text healthText;

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip healSound;

    private bool invincible = false;
    private SpriteRenderer sr;
    private AudioSource audioSource;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        UpdateHealthUI();
    }

    void Update()
    {
        if (transform.position.y < fallLimit)
        {
            RestartLevel();
        }
    }

    public void TakeDamage(int damage)
    {
        if (invincible)
            return;

        hp -= damage;
        UpdateHealthUI();

        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        if (hp <= 0)
        {
            RestartLevel();
            return;
        }

        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        invincible = true;

        for (int i = 0; i < 6; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.15f);
        }

        sr.enabled = true;
        invincible = false;
    }

    public void Heal(int amount)
    {
        int oldHp = hp;

        hp = Mathf.Min(3, hp + amount);
        UpdateHealthUI();

        if (hp > oldHp)
        {
            if (audioSource != null && healSound != null)
            {
                audioSource.PlayOneShot(healSound);
            }
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + hp + "/3";
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
