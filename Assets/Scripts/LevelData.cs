
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "LevelData",
    menuName = "Shadow Slayer/Data/Level"
)]
public class LevelData : ScriptableObject
{
    [Header("Level Information")]
    public string levelName;
    public string sceneName;
    public int levelNumber;

    [Header("Player Settings")]
    public Vector2 playerSpawnPosition;
    public int startingLives = 3;

    [Header("Level Goals")]
    public int targetCrystals = 5;
    public string goalDescription = "Collect crystals and reach the exit";

    [Header("Time Settings")]
    public float timeLimitSeconds = 180f;

    [Header("Enemy Configuration")]
    public List<EnemyData> enemyTypes = new List<EnemyData>();
    public int waveCount = 1;
}
