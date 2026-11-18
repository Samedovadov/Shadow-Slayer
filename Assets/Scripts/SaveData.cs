
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [Header("Progress")]
    public int highestUnlockedLevel = 1;
    public List<int> completedLevels = new List<int>();
    public int bestScore = 0;

    [Header("Player Data")]
    public int savedHealth = 3;
    public int savedCrystals = 0;

    [Header("Level Checkpoint")]
    public string currentLevelScene = "";
    public int levelStartHealth = 3;
    public int levelStartCrystals = 0;

    [Header("Audio Settings")]
    public float musicVolume = 0.6f;
    public float sfxVolume = 0.8f;
    public bool muteAll = false;
}
