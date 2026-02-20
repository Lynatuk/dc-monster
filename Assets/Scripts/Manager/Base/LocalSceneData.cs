using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LocalSceneData
{
    private Dictionary<string, Queue<string>> scenesSettingsData;
    
    public double serverTime;
    public double serverEndDayTime;

    public LocalSceneData()
    {
        scenesSettingsData = new Dictionary<string, Queue<string>>();
    }

    public void Flush(string sceneName)
    {
        RemoveData(sceneName);
    }

    public static T GetObject<T>(string sceneName)
    {
        T obj;
        string str = Data.Instance.localScenesData.GetData(sceneName);
        obj = JsonUtility.FromJson<T>(str);
        
        return obj;
    }

    public string GetData(string key)
    {
        string str1 = "";
        
        foreach (var keys in scenesSettingsData.Keys) 
        {
            str1 += keys + "  \n";
        } 
        
        string str="";

        if (scenesSettingsData.ContainsKey(key))
        {
            if (scenesSettingsData[key].Count > 0)
            {
                str = scenesSettingsData[key].Dequeue();

            }
            else 
            {
                RemoveData(key);

            }
        }
        else 
        {
           throw new Exception(key + " doesn't contains ");
        }
        
        return str;
    }

    public void RemoveData(string key)
    {
        scenesSettingsData.Remove(key);
    }

    /// <summary>
    /// Adds data associated with the scene to the storage
    /// </summary>
    /// <param name="key">Имя сцены</param>
    /// <param name="value">данные</param>
    public void SetData(string key,string value)
    {
        if (scenesSettingsData.ContainsKey(key))
        {
            scenesSettingsData[key].Enqueue(value);
        }
        else 
        {
            scenesSettingsData.Add(key, new Queue<string>());
            scenesSettingsData[key].Enqueue(value);
        }
        
        string str1 = "";
        
        foreach (var keys in scenesSettingsData.Keys)
        {
            str1 += keys + "  \n";
        }
    }
}