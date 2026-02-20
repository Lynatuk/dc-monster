using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class Data : ModuleInstance<Data>
{
    public LocalSceneData localScenesData;
    public LocalData localData;

    private string _pathToLocalData;
    private readonly string _localSaveFileName = "local.txt";

    private FileStream _fileStreamLocal;

    protected override void Init()
    {
        _pathToLocalData = Application.temporaryCachePath + "/ld";

        instance.localScenesData = new LocalSceneData();

        LoadLocalData();

        Check(_pathToLocalData);

        MarkAsInitialized();
    }

    private void SaveLocalDataToFile()
    {
        if (instance != null) {
            var path = _pathToLocalData + $"/{_localSaveFileName}";
            string json = JsonConvert.SerializeObject(instance.localData);
            File.WriteAllText(path, json);
        }
    }

    private void LoadLocalData()
    {
        string json = "";
        var path = _pathToLocalData + $"/{_localSaveFileName}";

        Check(_pathToLocalData);

        _fileStreamLocal = new FileStream(path,
            FileMode.OpenOrCreate,
            FileAccess.ReadWrite,
            FileShare.None);

        _fileStreamLocal.Dispose();

        json = File.ReadAllText(path);

        instance.localData = new LocalData();
        localData.settings = new LocalData.Settings();

        if (!string.IsNullOrEmpty(json) && json != "")
        {
            instance.localData = JsonConvert.DeserializeObject<LocalData>(json);
        }
    }

    private static void Check(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private void OnApplicationQuit()
    {
        SaveLocalDataToFile();
    }

    private void OnApplicationPause(bool pause)
    {
        SaveLocalDataToFile();
    }
}