using DG.Tweening;
using UnityEngine;

public class ProgressSliderController : MonoBehaviour
{

    public ProgressSliderUI progressSliderUI;

    public void SetProgress(int current, int max)
    {
        progressSliderUI.currentProgress.SetText("{0}", current);
        progressSliderUI.targetProgress.SetText("{0}", max);
        progressSliderUI.sliderProgress.value = (float)current / max;
    }

    public void UpdateProgress(int current, int max)
    {
        progressSliderUI.currentProgress.SetText("{0}", current);
        progressSliderUI.targetProgress.SetText("{0}", max);
        progressSliderUI.sliderProgress.DOValue((float)current / max - 0.01f, 0.3f).SetEase(Ease.OutCubic).SetDelay(0.2f);
    }

}
