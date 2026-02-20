using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;

public class Popup : BaseWindow
{

    [SerializeField] private Button _exitButton;

    [SerializeField] private WindowTypeAnim animOnStart;
    [SerializeField] private WindowTypeAnim animOnExit;
    [SerializeField] private List<Animator> animators;
    [SerializeField] private List<CanvasGroup> _canvasGroup;

    [SerializeField] private float _animationTime = 0.3f;

    private void Start()
    {
        OnOpen();
    }

    public override void OnOpen()
    {
        nameWindow = SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name;

        Navigation.AddWindow(this);

        if (_exitButton)
        {
            _exitButton.onClick.AddListener(() => 
            { 
                OnClose();
                _exitButton.onClick.RemoveAllListeners();
            });
        }

        PlayAnimWindow(animOnStart, WindowAnim.OpenUpWindow);
    }

    public override void OnClose(UnityAction actionClose = null)
    {
        PlayAnimWindow(animOnExit, WindowAnim.CloseUpWindow);

        OnWindowClosed += actionClose;
        OnWindowClosed?.Invoke();

        Navigation.UnloadScene(nameWindow, _animationTime);
    }

    void PlayAnimWindow(WindowTypeAnim windowTypeAnim, WindowAnim windowAnim)
    {
        switch (windowTypeAnim)
        {
            case WindowTypeAnim.WithoutAnim:

                break;
            case WindowTypeAnim.FadeCanvas:
                if (windowAnim == WindowAnim.OpenUpWindow)
                {
                    FadeWindow(0f, 1f, _animationTime);
                }
                else if (windowAnim == WindowAnim.CloseUpWindow)
                {
                    FadeWindow(1f, 0f, _animationTime);
                }
                break;
            case WindowTypeAnim.Animator:
                if (Navigation.TransitionsWindow == TransitionsWindow.Close)
                {
                    foreach (Animator animator in animators)
                    {
                        animator.enabled = true;
                        animator.Play(windowAnim.ToString());
                        _animationTime = GetAnimationDuration(animator, windowAnim.ToString());
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FadeWindow(float startValue, float endValue, float timeToFade)
    {
        for (int i = 0; i < _canvasGroup.Count; i++)
        {
            FadeCanvasGroup(_canvasGroup[i], startValue, endValue, timeToFade);
        }
    }

    void FadeCanvasGroup(CanvasGroup canvasGroup, float startValue, float endValue, float timeToFade)
    {
        canvasGroup.alpha = startValue;
        canvasGroup.DOFade(endValue, timeToFade).SetUpdate(true);
    }

    public float GetAnimationDuration(Animator animator, string animationName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return 0f;
    }
}

[SerializeField]
public enum WindowTypeAnim
{
    WithoutAnim,
    FadeCanvas,
    Animator
}

[SerializeField]
public enum WindowAnim
{
    OpenUpWindow,
    CloseUpWindow
}