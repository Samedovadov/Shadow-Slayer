
using UnityEngine;

public class SaveOnPause : MonoBehaviour
{
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveCurrentData();
        }
    }

    void OnApplicationQuit()
    {
        SaveCurrentData();
    }

    void SaveCurrentData()
    {
        SaveData data = SaveSystem.Load();

        PlayerHealth playerHealth =
            FindFirstObjectByType<PlayerHealth>();

        PlayerMovement playerMovement =
            FindFirstObjectByType<PlayerMovement>();

        if (playerHealth != null)
            data.savedHealth = playerHealth.HP;

        if (playerMovement != null)
            data.savedCrystals = playerMovement.Crystals;

        SaveSystem.Save(data);

        Debug.Log("Game data saved on pause or quit.");
    }
}
