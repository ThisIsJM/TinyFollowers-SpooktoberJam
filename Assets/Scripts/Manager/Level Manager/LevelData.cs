using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public List<int> SceneLevelChapter { get; set; }
    public List<string> SceneLevelName { get; set; }
    public List<bool> SceneLevelStatus { get; set; }

    public static LevelData FromLevelManager(LevelManager levelManager)
    {
        return new LevelData
        {
            SceneLevelChapter = levelManager.GetSceneLevelChapter(),
            SceneLevelName = levelManager.GetSceneLevelName(),
            SceneLevelStatus = levelManager.GetSceneLevelStatus()
        };
       
    }
}

