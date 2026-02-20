using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Utility : ModuleInstance<Utility>
{
    private static UnityAction onSequenceRunningSucceeded;
    public static UnityAction OnActionEnded = () =>
    {
        if (instance != null)
        {
            if (instance.isActionEnded != null)
            {
                if (instance.isActionEnded.Count != 0)
                {
                    if (instance._currentAction < instance.isActionEnded.Count)
                    {
                        instance.isActionEnded[instance._currentAction] = true;
                    }
                }
            }
        }

    };

    private int _currentAction;
    [HideInInspector]
    public List<bool> isActionEnded;

    protected override void Init()
    {
        MarkAsInitialized();
    }

    /// <summary>
    /// Run the action in 1 sec by default
    /// </summary>
    /// <param name="action"></param>
    public static void RunWithDelay(UnityAction action)
    {
        instance.StartCoroutine(instance.Routine(0, action));
    }
    public static Coroutine RunWithDelay(float delay, UnityAction action)
    {
        if (instance)
        {
            var coroutineWithDelay = instance.StartCoroutine(instance.Routine(delay, action));

            return coroutineWithDelay;
        }

        return null;
    }

    public static void StopRunWithDelay(Coroutine coroutineWithDelay)
    {
        if (instance != null)
        {
            instance.StopCoroutine(coroutineWithDelay);
        }
    }

    public static void StopRoutine()
    {
        instance.StopAllCoroutines();
    }

    private IEnumerator Routine(float sec, UnityAction action)
    {
        yield return new WaitForSecondsRealtime(sec);
        action?.Invoke();
    }

    private IEnumerator Qroutine(params UnityAction[] actions)
    {
        foreach (var action in actions)
        {
            yield return null;
            action?.Invoke();
        }

        yield return null;
    }

    public static void RunQueue(params UnityAction[] actions)
    {
        instance.StartCoroutine(instance.Qroutine(actions));
    }

    public static void StartStaticCoroutine(IEnumerator iEnumerator)
    {
        instance.StartCoroutine(iEnumerator);
    }

    /// <summary>
    /// Returns a unique id of gameObject (i hope)
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static string GetUniqueId(GameObject go)
    {
        string id;
        if (go.transform.parent != null)
        {
            id = go.gameObject.name + go.transform.parent.name + go.transform.parent.GetSiblingIndex() + "_" + go.transform.parent.childCount;
        }
        else
        {
            id = go.gameObject.name;

        }
        id += go.gameObject.transform.childCount.ToString() + "_" + go.transform.GetSiblingIndex();
        return id;
    }

    /// <summary>
    /// Выполняет последовательность действий 
    /// Важно! В конце действия обязательно вызывать событие OnActionEnded 
    /// </summary>
    /// <param name="actions">Методы</param>
    public static void TryRunActionSequence(params UnityAction[] actions)
    {
        instance.isActionEnded = new List<bool>();
        foreach (var action in actions)
        {
            instance.isActionEnded.Add(false);
        }
        
        instance.StartCoroutine(instance.RunActionSequence(actions));
    }
    
    private IEnumerator RunActionSequence(UnityAction[] actions)
    {
        instance._currentAction = 0;
        bool[] isActionRunning = new bool[actions.Length];
        for (int i = 0; i < isActionRunning.Length; i++)
        {
            isActionRunning[i] = false;
        }
        actions[_currentAction]();
        isActionRunning[_currentAction] = true;

        while (!instance.Check())
        {
            yield return new WaitForSeconds(0.5f);
            if (instance.isActionEnded[_currentAction])
            {
                _currentAction++;
            }
            else
            {
                if (isActionRunning[_currentAction])
                {
                    yield return null;
                }
                else
                {
                    actions[_currentAction]();
                    isActionRunning[_currentAction] = true;

                }
            }
        }
        if (onSequenceRunningSucceeded != null)
        {
            onSequenceRunningSucceeded();
            onSequenceRunningSucceeded = null;
        }
        yield break;
    }
    
    //public static bool IsPlayer(int id) => (Data.instance.global.playerData.id == id);
    private bool Check()
    {
        bool ret = true;
        foreach (var b in instance.isActionEnded)
        {
            if (!b)
            {
                ret = false;
            }
        }
        
        return ret;
    }
    
    public static double GetSecSince(DateTime time)
    {
        double sec = 0;
        var timeSpan = DateTime.Now.Subtract(time);
        sec = timeSpan.TotalSeconds;
        
        return sec;
    }

    public static void GetAllObjects()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
    }

    public static float GetAnimationLength(Animator animator, string animationName)
    {
        var animatorController = animator.runtimeAnimatorController;

        foreach (var animation in animatorController.animationClips)
        {
            if (animation.name == animationName)
            {
                return animation.length;
            }
        }

        throw new Exception("Unknown animation name");
    }
}
