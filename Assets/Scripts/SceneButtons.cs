
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtons : MonoBehaviour
{
    [Header("Reset Confirmation")]
    public GameObject resetConfirmPanel;

    public void PlayGame()
    {
        SaveData data = SaveSystem.Load();

        int level = 1;

        if (data.completedLevels.Contains(1))
            level = 2;

        if (data.completedLevels.Contains(2))
            level = 3;

        if (data.completedLevels.Contains(3))
            level = 1;

        Debug.Log("Completed levels: " +
            string.Join(", ", data.completedLevels));

        Debug.Log("Loading level: " + level);

        SceneManager.LoadScene("Level" + level);
    }

    public void OpenResetConfirmation()
    {
        if (resetConfirmPanel != null)
            resetConfirmPanel.SetActive(true);
    }

    public void CancelReset()
    {
        if (resetConfirmPanel != null)
            resetConfirmPanel.SetActive(false);
    }

    public void ConfirmReset()
    {
        SaveData oldData = SaveSystem.Load();

        float music = oldData.musicVolume;
        float sfx = oldData.sfxVolume;
        bool mute = oldData.muteAll;

        SaveSystem.ResetProgress();

        SaveData newData = new SaveData();

        newData.musicVolume = music;
        newData.sfxVolume = sfx;
        newData.muteAll = mute;

        SaveSystem.Save(newData);

        Debug.Log("Progress reset. Audio settings preserved.");

        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAgain()
    {
        ConfirmReset();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
