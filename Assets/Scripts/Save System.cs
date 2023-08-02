using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveSystem
{
    public static readonly string path = Path.Combine(Application.persistentDataPath, "level.sav");

    public static void SaveLevel(LevelManager levelManager)
    {
        FileStream stream = new FileStream(path, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(stream);

        LevelData data = LevelData.FromLevelManager(levelManager);

        writer.Write(data.SceneLevelName.Count); //Write the length of the Chapter List
        foreach (int sceneChapter in data.SceneLevelChapter) //Writes the chapter of the Level
        {
            writer.Write(sceneChapter);
        }

        writer.Write(data.SceneLevelName.Count); //Write the length of the Scene Name List
        foreach (string sceneName in data.SceneLevelName) //Writes the name of the Level
        {
            writer.Write(sceneName);
        }

        writer.Write(data.SceneLevelStatus.Count);
        foreach (bool sceneStatus in data.SceneLevelStatus) //Writes the status of the Level
        {
            writer.Write(sceneStatus);
        }

        stream.Close();
    }

    public static LevelData LoadLevel()
    {
        if (!File.Exists(path))
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }

        FileStream stream = new FileStream(path, FileMode.Open);
        BinaryReader reader = new BinaryReader(stream);

        LevelData data = new LevelData();
        data.SceneLevelChapter = new List<int>();
        data.SceneLevelName = new List<string>();
        data.SceneLevelStatus = new List<bool>();

        //Must read the Binary Data in the same order it was read, to avoid weird errors
        int sceneChapterLength = reader.ReadInt32();// Debug.Log(sceneChapterLength);
        for (int i = 0; i < sceneChapterLength; i++) //For the SceneNameList
        {
            data.SceneLevelChapter.Add(reader.ReadInt32());
        }

        int sceneNameLength = reader.ReadInt32(); Debug.Log(sceneNameLength);
        for (int i = 0; i < sceneNameLength; i++) //For the SceneNameList
        {
            data.SceneLevelName.Add(reader.ReadString());
        }

        int sceneStatusLength = reader.ReadInt32();

        for (int i = 0; i < sceneStatusLength; i++) //For the SceneNameList
        {
            data.SceneLevelStatus.Add(reader.ReadBoolean());
        }

        stream.Close();
        return data;

    }
}
