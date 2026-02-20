using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorCacheManager : EditorWindow
{
    [MenuItem("Cache/Clear all")]
    static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("CACHE Directory was deleted"+Application.persistentDataPath);
        Debug.Log("CACHE Directory was deleted"+ Application.temporaryCachePath);
        if (Directory.Exists(Application.persistentDataPath)) {
            Directory.Delete(Application.persistentDataPath, true);
        }
        if (Directory.Exists(Application.temporaryCachePath))
        {
            Directory.Delete(Application.temporaryCachePath, true);
        }
    }
    [MenuItem("Cache/Log localData")]
    static void ShowLocalData()
    {
        string pathToLocal = Application.persistentDataPath+"/ld";
        if (Directory.Exists(Application.persistentDataPath))
        {
            if (Directory.Exists(pathToLocal)) {
                if (File.Exists(pathToLocal + "/ld")) {
                    string str = File.ReadAllText(pathToLocal + "/ld");
                    Debug.Log(str);
                }
            }
        }
       
    }

    [MenuItem("Cache/Log Core Data")]
    static void ShowCoreData()
    {
        string pathToLocal = Application.persistentDataPath + "/ld";
        if (Directory.Exists(Application.persistentDataPath))
        {
            if (Directory.Exists(pathToLocal))
            {
                if (File.Exists(pathToLocal + "/ubld"))
                {
                    string str = File.ReadAllText(pathToLocal + "/ubld");
                }
            }
        }

    }

 

}
