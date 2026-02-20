using System;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Navigation : ModuleInstance<Navigation>
{
    private static List<Scene> loadedScene;

    public List<BaseWindow> baseWindows;

    private Stack<string> _scenesSequenseQueue;
    private Queue<string> _scenesSequence;

    private Dictionary<string, UnityAction> _onLoadActions;
    private Dictionary<string, UnityAction> _onUnloadActions;

    public bool delayOpenScene = false;
    public bool delayCloseScene = false;

    public static TransitionsWindow TransitionsWindow;

    protected override void Init()
    {
        SceneManager.sceneLoaded += instance.OnLoaded;
        SceneManager.sceneUnloaded += instance.OnSceneUnloaded;

        _scenesSequenseQueue = new Stack<string>();
        baseWindows = new List<BaseWindow>();
        loadedScene = new List<Scene>();
        instance._onLoadActions = new Dictionary<string, UnityAction>();
        instance._onUnloadActions = new Dictionary<string, UnityAction>();

        instance._scenesSequence = new Queue<string>();

        MarkAsInitialized();
    }

    #region OpenWindow
    public static void ShowWindow(string sceneName, LoadSceneMode loadWindowMode = LoadSceneMode.Additive, float delayBeforeNextOpening = 0.3f)
    {
        if (!GetDelayOpenScene())
        {
            if (loadWindowMode == LoadSceneMode.Additive && !IsSceneOpened(sceneName))
            {
                SetDelayOpeningScene(delayBeforeNextOpening);

                LoadScene(sceneName, loadWindowMode);
            }
            else if (loadWindowMode == LoadSceneMode.Single)
            {
                SetDelayOpeningScene(delayBeforeNextOpening);

                FlushActionsClearAll();

                LoadScene(sceneName, loadWindowMode);
            }
        }
    }

    static void LoadScene(string nameScene, LoadSceneMode loadSceneMode)
    {
        SceneManager.LoadSceneAsync(nameScene, loadSceneMode).completed += (_) =>
        {
            Scene scene = SceneManager.GetSceneByName(nameScene);
            loadedScene.Add(scene);

            if (instance._scenesSequence.Contains(scene.name))
            {
                instance._scenesSequenseQueue.Push(scene.name);
            }
        };
    }

    public static void AddWindow(BaseWindow baseWindow)
    {
        instance.baseWindows.Add(baseWindow);
    }
    #endregion

    #region CloseWindow
    public static void Back(string nameScene = "", bool forcedBack = false)
    {
        if (instance.baseWindows.Count > 1 && (!GetDelayCloseScene() || forcedBack) && IsSceneOpened(nameScene))
        {
            PeekBaseWindow(nameScene).OnClose();
        }
    }
    
    public static void CloseNScenes(int countScenes)
    {
        for (int i = 0; i < countScenes; i++)
        {
            Back();
        }
    }

    public static void CloseAll()
    {
        for (int i = 0; i < instance.baseWindows.Count; i++)
        {
            Utility.RunWithDelay((i - 1) / 10f, () => Back());
        }
    }

    public static void UnloadScene(string nameScene = "", float delayClosing = 0)
    {
        if (loadedScene.Count > 0)
        {
            Scene unloadedScene = GetSceneInstance(nameScene);

            instance.delayCloseScene = true;

            Utility.RunWithDelay(delayClosing, () =>
            {
                SceneManager.UnloadSceneAsync(unloadedScene).completed += (asyncHandle) =>
                {
                    loadedScene.Remove(unloadedScene);
                    PopBaseWindow(unloadedScene.name);

                    if (instance._scenesSequenseQueue.Count > 0)
                    {
                        instance._scenesSequenseQueue.Pop();
                    }

                    instance.delayCloseScene = false;
                };
            });
        }
    }
    #endregion

    #region AuxiliaryMethods
    public static void FlushActionsClearAll()
    {
        if (instance.baseWindows.Count > 0)
        {
            PopBaseWindow();
        }

        instance._onLoadActions.Clear();
        instance._onUnloadActions.Clear();

        loadedScene.Clear();
        instance._scenesSequence.Clear();
        instance.baseWindows.Clear();
    }

    public static void ClearSceneToQueue()
    {
        instance.baseWindows.Clear();
        instance._scenesSequence.Clear();
    }

    public static void AddExitActionOnWindow(string nameWindow, UnityAction action)
    {
        PeekBaseWindow(nameWindow).AddOnWindowClosedEvent(action);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (instance._onUnloadActions.ContainsKey(scene.name))
        {
            instance._onUnloadActions[scene.name]();
            instance._onUnloadActions.Remove(scene.name);
        }
    }

    private void OnLoaded(Scene scene, LoadSceneMode mode)
    {
        if (instance._onLoadActions.ContainsKey(scene.name))
        {
            instance._onLoadActions[scene.name]();
            instance._onLoadActions.Remove(scene.name);
        }
    }

    public static void AddOnLoadEvent(string SceneName, UnityAction action, LoadWindowMode mode)
    {
        if (mode == LoadWindowMode.Additive)
        {
            if (instance._onLoadActions.ContainsKey(SceneName))
            {
                instance._onLoadActions[SceneName] += action;
            }
            else
            {
                instance._onLoadActions.Add(SceneName, action);
            }
        }
        else
        {
            if (instance._onLoadActions.ContainsKey(SceneName))
            {
                instance._onLoadActions[SceneName] = null;
                instance._onLoadActions[SceneName] = action;
            }
            else
            {
                instance._onLoadActions.Add(SceneName, action);
            }
        }
    }

    public static void AddOnUnloadEvent(string SceneName, UnityAction action, LoadWindowMode mode)
    {
        if (mode == LoadWindowMode.Additive)
        {
            if (instance._onUnloadActions.ContainsKey(SceneName))
            {
                instance._onUnloadActions[SceneName] += action;
            }
            else
            {
                instance._onUnloadActions.Add(SceneName, action);
            }
        }
        else
        {
            if (instance._onUnloadActions.ContainsKey(SceneName))
            {
                instance._onUnloadActions[SceneName] = null;
                instance._onUnloadActions[SceneName] = action;
            }
            else
            {
                instance._onUnloadActions.Add(SceneName, action);
            }
        }
    }

    public static void SetDelayOpeningScene(float delay)
    {
        if (delay > 0)
        {
            instance.delayOpenScene = true;
            Utility.RunWithDelay(delay, () => instance.delayOpenScene = false);
        }
        else if (delay == 0)
        {
            instance.delayOpenScene = false;
        }
    }

    #endregion

    #region ReturnedMethods
    public static string GetNameLastLoadScene()
    {
        var lastScene = GetLastLoadedWindowInfo();

        if (lastScene != null)
        {
            return GetLastLoadedWindowInfo().nameWindow;
        }
        else
        {
            return "";
        }
    }

    public static BaseWindow GetLastLoadedWindowInfo()
    {
        return PeekBaseWindow();
    }

    private static BaseWindow PopBaseWindow(string nameScene = "")
    {
        BaseWindow baseWindow;

        if (instance.baseWindows.Count == 0)
        {
            return null;
        }
        else
        {
            if (nameScene == "")
            {
                baseWindow = instance.baseWindows[instance.baseWindows.Count - 1];
            }
            else
            {
                baseWindow = instance.baseWindows.Find(name => name.nameWindow == nameScene);
            }

            instance.baseWindows.Remove(baseWindow);
            instance.baseWindows.RemoveAll(item => item == null);

            return baseWindow;
        }
    }

    private static BaseWindow PeekBaseWindow(string nameScene = "")
    {
        if (instance.baseWindows.Count == 0)
        {
            return null;
        }
        else
        {
            BaseWindow baseWindow;
            if (nameScene == "")
            {
                baseWindow = instance.baseWindows[instance.baseWindows.Count - 1];
            }
            else
            {
                baseWindow = instance.baseWindows.Find(name => name.nameWindow == nameScene);
            }

            return baseWindow;
        }
    }

    private static Scene GetSceneInstance(string nameScene = "")
    {
        if (loadedScene.Count == 0)
        {
            return new Scene();
        }

        if (nameScene == "")
        {
            return loadedScene[loadedScene.Count - 1];
        }
        else
        {
            return loadedScene.Find(sceneInfo => sceneInfo.name == nameScene);
        }
    }

    public static bool CheckOpenedScene(string sceneName)
    {
        return IsSceneOpened(sceneName) || instance._scenesSequence.Contains(sceneName);
    }

    public static bool IsWindowsListEmptyAndSequence()
    {
        if (IsSceneSequenceEmpty() && IsWindowsListEmpty())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsWindowsListEmpty()
    {
        if (instance.baseWindows.Count <= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private static bool GetDelayOpenScene() => instance.delayOpenScene;
    private static bool GetDelayCloseScene() => instance.delayCloseScene;
    private static bool IsSceneSequenceEmpty() => instance._scenesSequence.Count == 0;
    private static bool IsSceneOpened(string sceneName) => GetSceneInstance(sceneName).isLoaded;
    #endregion
}

[Serializable]
public enum LoadWindowMode
{
    Single = 0,
    Additive = 1
}

[Serializable]
public enum TransitionsWindow
{
    Close,
    Transition
}