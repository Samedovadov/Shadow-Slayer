
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [Header("Level Data")]
    public LevelData levelData;

    [Header("UI")]
    public TMP_Text timerText;

    private float remainingTime;
    private bool timeIsOver = false;

    void Start()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelTimer: LevelData is not assigned!");
            enabled = false;
            return;
        }

        remainingTime = levelData.timeLimitSeconds;

        if (remainingTime <= 0)
        {
            Debug.LogWarning("LevelTimer: Time limit is disabled.");
            enabled = false;
            return;
        }

        UpdateTimerUI();
    }

    void Update()
    {
        if (timeIsOver)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            timeIsOver = true;
            UpdateTimerUI();

            SoundEffects.Play(SoundEffects.Cue.Lose);
            Debug.Log("TIME IS OVER! Restarting level.");

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );

            return;
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        timerText.text = string.Format(
            "{0:00}:{1:00}",
            minutes,
            seconds
        );
    }
}
