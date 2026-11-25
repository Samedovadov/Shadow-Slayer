using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    [Header("Audio Snapshots")]
    [SerializeField] private AudioMixerSnapshot normalSnapshot;
    [SerializeField] private AudioMixerSnapshot pausedSnapshot;

    void Start()
    {
        ResumeGame();
    }

    public void PauseGame()
    {
        if (pausedSnapshot != null)
        {
            pausedSnapshot.TransitionTo(0f);
        }

        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        RestoreNormalAudio();
        pausePanel.SetActive(false);
    }

    public void RestartGame()
    {
        RestoreNormalAudio();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        RestoreNormalAudio();
        SceneManager.LoadScene("MainMenu");
    }

    private void RestoreNormalAudio()
    {
        Time.timeScale = 1f;

        if (normalSnapshot != null)
        {
            normalSnapshot.TransitionTo(0f);
        }
    }
}