using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public abstract class Module : MonoBehaviour
{
    [SerializeField] private int priority;

    protected abstract void Init();

    public abstract void Initialize();

    public int GetPriority() => priority;
}

public class ModuleInstance<T> : Module where T : Component
{
    private Stopwatch sw;

    private static bool initialized;
    public static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                throw new Exception("You trying to get access. before the initialization of module " + typeof(T).ToString() + " completed");
            }
            else
            {
                return instance;
            }
        }
        set => instance = value as T;
    }

    protected override void Init()
    {
        //Debug.LogError("Not developed yet");
    }

    public override void Initialize()
    {
        sw = new Stopwatch();
        sw.Start();

        if (!instance)
        {
            if (transform.parent)
            {
                throw new Exception("Singleton must be 1 in hierarchy");
            }
            else
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
                Init();
                StartCoroutine(WaitUntillInitialized());
            }
        }
    }

    private IEnumerator WaitUntillInitialized()
    {
        yield return new WaitUntil(() => initialized == true);

        ModuleManager.OnInitialized(this);
    }

    protected void MarkAsInitialized()
    {
        try
        {
            sw.Stop();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("NULL SW :" + e.StackTrace);
        }

        initialized = true;
    }
}