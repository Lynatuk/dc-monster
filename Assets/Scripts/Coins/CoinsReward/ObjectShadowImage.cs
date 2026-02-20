using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ObjectShadowImage : MonoBehaviour
{
    [SerializeField] private Image shadow;

    [SerializeField] private float alphaMax;
    [SerializeField] private float duration;

    private Tween shadowTween;

    public void ShowShadow()
    {
        StartAnim(alphaMax, duration);
    }

    public void HideShadow()
    {
        StartAnim(0, duration);
    }

    private void StartAnim(float endAlpha, float dur)
    {
        shadowTween?.Kill();
        shadowTween = shadow.DOFade(endAlpha, dur).SetLink(gameObject);
    }
}
