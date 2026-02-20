using UnityEngine;
using UnityEngine.Events;

public abstract class BaseWindow : MonoBehaviour
{

    [HideInInspector]
    public string nameWindow;

    protected UnityAction OnWindowClosed;

    public void AddOnWindowClosedEvent(UnityAction actionClosed)
    {
        OnWindowClosed += actionClosed;
    }
    
    public abstract void OnOpen();

    public abstract void OnClose(UnityAction actionClose = null);
}