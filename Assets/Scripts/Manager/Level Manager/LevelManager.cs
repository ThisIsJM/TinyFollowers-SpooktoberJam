using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;

public class LevelManager : MonoBehaviour
{
    [Header("Scene Levels")]
    public List<SceneReference> chapterOneLevelList;
    public List<SceneReference> chapterTwoLevelList;

    private List<SceneLevel> sceneLevelList;

    public static LevelManager instance;

    public string currentChapter;

    public int restartCount;

    private void Awake()
    {
        if (instance == null)
        { instance = this; }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        LoadLevelData();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += UnlockLevel;
    }

    public List<SceneLevel> CreateLevelData(List<SceneReference> sceneObjectList)
    {
        List<SceneLevel> sceneLevelList = new List<SceneLevel>();

        for (int i = 0; i < sceneObjectList.Count; i++)
        {
            int chapter = FindLevelChapter(sceneObjectList[i].ScenePath);

            sceneLevelList.Add(new SceneLevel(chapter, sceneObjectList[i].ScenePath, false));
        }

        sceneLevelList[0] = new SceneLevel(1, sceneObjectList[0].ScenePath, true);
        return sceneLevelList;
    }

    public int FindLevelChapter(string scenePath)
    {
        string currentScenePath = "";
        foreach (SceneReference obj in chapterOneLevelList)
        {
            if (obj.ScenePath.Equals(scenePath))
            {
                return 1;
            }
            else
            {
                currentScenePath = obj.ScenePath;
            }
        }

        //Debug.LogError("Object SceneName is: " + currentScenePath + "While sceneName is " + scenePath);
        return 2;
    }

    public bool InALevelScene(string currentScenePath)
    {
        foreach (SceneLevel sceneLevel in sceneLevelList)
        {
            if (sceneLevel.sceneName.Equals(currentScenePath))
            {
                return true;
            }
        }
        return false;
    }

    private string SplitScenePath(string ScenePath)
    {
        string[] split = ScenePath.Split('/');
        string sceneName = split[split.Length - 1].Replace(".unity", "");

        return sceneName;
    }

    public void SaveLevelData()
    {
        SaveSystem.SaveLevel(this);
    }

    public void LoadLevelData()
    {
        LevelData data = SaveSystem.LoadLevel();

        if (data == null)
        {
            List<SceneReference> chapterList = new List<SceneReference>();

            chapterList.AddRange(chapterOneLevelList);
            chapterList.AddRange(chapterTwoLevelList);

            sceneLevelList = CreateLevelData(chapterList);
        }
        else
        {
            List<SceneLevel> sceneLevelListFromSave = new List<SceneLevel>();

            for (int i = 0; i < data.SceneLevelName.Count; i++)
            {
                sceneLevelListFromSave.Add(new SceneLevel(data.SceneLevelChapter[i], data.SceneLevelName[i], data.SceneLevelStatus[i]));
            }

            sceneLevelList = sceneLevelListFromSave;
        }
    }

    public void UnlockLevel(Scene scene, LoadSceneMode mode)
    {
        for (int i = 0; i < sceneLevelList.Count; i++)
        {
            if (sceneLevelList[i].sceneName.Equals(scene.path) && !sceneLevelList[i].isUnlocked)
            {
                restartCount = 0;
                sceneLevelList[i] = new SceneLevel(sceneLevelList[i].chapter, sceneLevelList[i].sceneName, true);
                Debug.Log("Unlocked Level " + sceneLevelList[i].sceneName);
                SaveLevelData();
                return;
            }
        }
    }

    public List<SceneLevel> GetSceneLevelList()
    {
        Debug.Log(sceneLevelList.Count);
        return sceneLevelList;
    }

    public List<int> GetSceneLevelChapter()
    {
        List<int> sceneChapterList = new List<int>();

        foreach (SceneLevel sceneLevel in sceneLevelList)
        {
            sceneChapterList.Add(sceneLevel.chapter);
        }

        return sceneChapterList;
    }

    public List<string> GetSceneLevelName()
    {
        List<string> sceneNameList = new List<string>();

        foreach (SceneLevel sceneLevel in sceneLevelList)
        {
            sceneNameList.Add(sceneLevel.sceneName);
        }

        return sceneNameList;
    }

    public List<bool> GetSceneLevelStatus()
    {
        List<bool> sceneNameList = new List<bool>();

        foreach (SceneLevel sceneLevel in sceneLevelList)
        {
            sceneNameList.Add(sceneLevel.isUnlocked);
        }

        return sceneNameList;
    }
}

public struct SceneLevel
{
    public int chapter;
    public string sceneName;
    public bool isUnlocked;

    public SceneLevel(int chapter, string sceneName, bool isUnlocked)
    {
        this.chapter = chapter;
        this.sceneName = sceneName;
        this.isUnlocked = isUnlocked;
    }
}



