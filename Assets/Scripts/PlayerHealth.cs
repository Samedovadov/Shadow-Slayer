
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

    public int HP => hp;

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip healSound;

    private bool invincible = false;
    private bool isRestarting = false;

    private SpriteRenderer sr;
    private AudioSource audioSource;
    private PlayerData playerData;

    private int MaxHealth => playerData != null
        ? Mathf.Max(1, playerData.maxHealth)
        : 3;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        PlayerMovement movement = GetComponent<PlayerMovement>();

        if (movement != null)
        {
            playerData = movement.playerData;
        }

        if (playerData != null)
        {
            fallLimit = playerData.fallLimit;
        }
        else
        {
            Debug.LogWarning("PlayerData not found!");
        }

        SaveData data = SaveSystem.Load();

        string sceneName = SceneManager.GetActiveScene().name;

        if (string.IsNullOrEmpty(data.currentLevelScene) &&
            data.completedLevels.Count == 0)
        {
            data.savedHealth = MaxHealth;
            data.savedCrystals = 0;
        }

        if (data.currentLevelScene != sceneName)
        {
            data.currentLevelScene = sceneName;
            data.levelStartHealth = data.savedHealth;
            data.levelStartCrystals = data.savedCrystals;
        }

        data.savedHealth = Mathf.Clamp(
            data.savedHealth,
            1,
            MaxHealth
        );

        data.levelStartHealth = Mathf.Clamp(
            data.levelStartHealth,
            1,
            MaxHealth
        );

        SaveSystem.Save(data);

        hp = data.savedHealth;

        UpdateHealthUI();
    }

    void Update()
    {
        if (!isRestarting && transform.position.y < fallLimit)
        {
            RestartLevel();
        }
    }

    public void TakeDamage(int damage)
    {
        if (invincible || isRestarting || damage <= 0)
            return;

        hp -= damage;
        UpdateHealthUI();

        if (audioSource != null && hurtSound != null)
        {
            SoundEffects.PlayVaried(hurtSound, audioSource.volume);
        }

        if (hp <= 0)
        {
            RestartLevel();
            return;
        }

        SavePlayerData();
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
        if (isRestarting || amount <= 0)
            return;

        int oldHp = hp;

        hp = Mathf.Min(MaxHealth, hp + amount);
        UpdateHealthUI();

        if (hp > oldHp)
        {
            if (audioSource != null && healSound != null)
            {
                SoundEffects.PlayVaried(healSound, audioSource.volume);
            }

            SavePlayerData();
        }
    }

    public void SetHealth(int amount)
    {
        hp = Mathf.Clamp(amount, 1, MaxHealth);
        UpdateHealthUI();
        SavePlayerData();
    }

    void SavePlayerData()
    {
        SaveData data = SaveSystem.Load();
        data.savedHealth = hp;
        SaveSystem.Save(data);
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + hp + "/" + MaxHealth;
        }
    }

    void RestartLevel()
    {
        if (isRestarting)
            return;

        isRestarting = true;
        SoundEffects.Play(SoundEffects.Cue.Death);

        SaveData data = SaveSystem.Load();

        data.savedHealth = Mathf.Clamp(
            data.levelStartHealth,
            1,
            MaxHealth
        );

        data.savedCrystals = data.levelStartCrystals;

        SaveSystem.Save(data);

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
