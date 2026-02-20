using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Window : BaseWindow
{
    private void Start()
    {
        OnOpen();
    }

    public override void OnOpen()
    {
        nameWindow = SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name;

        Navigation.AddWindow(this);
    }

    public override void OnClose(UnityAction actionClose = null)
    {
        OnWindowClosed += actionClose;
        OnWindowClosed?.Invoke();

        Navigation.UnloadScene(nameWindow);
    }
}