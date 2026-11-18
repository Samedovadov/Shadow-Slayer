
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            "save.json"
        );

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
            return new SaveData();

        try
        {
            string json = File.ReadAllText(SavePath);

            if (string.IsNullOrWhiteSpace(json))
                return new SaveData();

            SaveData data = new SaveData();

            JsonUtility.FromJsonOverwrite(json, data);

            if (data.completedLevels == null)
                data.completedLevels = new System.Collections.Generic.List<int>();

            if (data.currentLevelScene == null)
                data.currentLevelScene = "";

            data.highestUnlockedLevel =
                Mathf.Max(1, data.highestUnlockedLevel);

            data.bestScore = Mathf.Max(0, data.bestScore);
            data.savedCrystals = Mathf.Max(0, data.savedCrystals);
            data.levelStartCrystals = Mathf.Max(0, data.levelStartCrystals);

            data.savedHealth = Mathf.Max(1, data.savedHealth);
            data.levelStartHealth = Mathf.Max(1, data.levelStartHealth);

            data.musicVolume = Mathf.Clamp01(data.musicVolume);
            data.sfxVolume = Mathf.Clamp01(data.sfxVolume);

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "Save loading failed. Default data loaded: " + e.Message
            );

            return new SaveData();
        }
    }

    public static void Save(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError("Cannot save: SaveData is null!");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);

            File.WriteAllText(SavePath, json);

            Debug.Log("Game saved: " + SavePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save failed: " + e.Message);
        }
    }

    public static void ResetProgress()
    {
        try
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);

            Debug.Log("Save data deleted.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save reset failed: " + e.Message);
        }
    }
}
