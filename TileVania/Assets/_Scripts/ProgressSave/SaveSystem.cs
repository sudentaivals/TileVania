using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveData<T>(T data, string fileAdditionalpath)
    {
        var binaryFormetter = new BinaryFormatter();
        var path = Application.persistentDataPath + fileAdditionalpath;
        FileStream fs = new FileStream(path, FileMode.Create);

        binaryFormetter.Serialize(fs, data);

        fs.Close();
    }

    public static void ClearSaveData()
    {
        var path = Application.persistentDataPath;
        DirectoryInfo info = new DirectoryInfo(path);
        foreach (FileInfo file in info.GetFiles())
        {
            file.Delete();
        }
    }

    public static void SaveLevelData(LevelData data, int levelId)
    {
        var binaryFormetter = new BinaryFormatter();
        var path = Application.persistentDataPath + $"/Level{levelId}.tilevaniasg";
        FileStream fs = new FileStream(path, FileMode.Create);

        binaryFormetter.Serialize(fs,data);

        fs.Close();
    }

    public static void SaveLevelData(float time, int deaths, bool isCompleted, int levelId)
    {
        SaveLevelData(new LevelData(deaths, time, isCompleted), levelId);
    }

    public static LevelData GetLevelData(int levelId)
    {
        
        var path = Application.persistentDataPath + $"/Level{levelId}.tilevaniasg";
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);
            LevelData levelData = binaryFormatter.Deserialize(fs) as LevelData;
            fs.Close();
            return levelData;
        }
        else
        {
            SaveLevelData(0, 0, false, levelId);
            return GetLevelData(levelId);
        }
    }
}
