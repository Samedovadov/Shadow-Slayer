
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelExit : MonoBehaviour
{
    [Header("Level Settings")]
    public string nextScene = "Level2";
    public LevelData levelData;

    [Header("UI")]
    public TMP_Text requirementText;

    private bool isLoading = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isLoading)
            return;

        if (levelData == null)
        {
            Debug.LogError("LevelData is not assigned!");
            return;
        }

        PlayerMovement player =
            other.GetComponent<PlayerMovement>();

        if (player == null)
        {
            Debug.LogError("PlayerMovement not found!");
            return;
        }

        if (player.Crystals < levelData.targetCrystals)
        {
            int remaining =
                levelData.targetCrystals - player.Crystals;

            Debug.Log(
                "You need " + remaining + " more crystals!"
            );

            if (requirementText != null)
            {
                requirementText.text =
                    "Need " + remaining + " more crystals!";
            }

            return;
        }

        isLoading = true;

        if (requirementText != null)
        {
            requirementText.text = "";
        }

        SaveData data = SaveSystem.Load();

        int completedLevel = levelData.levelNumber;

        if (!data.completedLevels.Contains(completedLevel))
        {
            data.completedLevels.Add(completedLevel);
        }

        data.highestUnlockedLevel = Mathf.Max(
            data.highestUnlockedLevel,
            completedLevel + 1
        );

        SaveSystem.Save(data);

        Debug.Log("LEVEL SAVED: " + completedLevel);

        SceneManager.LoadScene(nextScene);
    }
}
