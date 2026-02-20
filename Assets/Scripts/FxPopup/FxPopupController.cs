using UnityEngine;
using DG.Tweening;

public class FxPopupController : MonoBehaviour
{
    public FxPopupUI fxPopupUI;

    [Header("AnimParams")]
    public float delayBeforeHide = 1;
    public float hideSpeed = 0.3f;
    public float upMovementTarget = 0.3f;

    private PopupPool _pool;

    public void Init(FxPopupInfo fxPopupInfo, PopupPool pool)
    {
        _pool = pool;

        fxPopupUI.canvasGroup.alpha = 0;
        fxPopupUI.canvasGroup.DOFade(1, hideSpeed);

        fxPopupUI.Setup(fxPopupInfo);

        ShowAnimate();
    }

    private void ShowAnimate()
    {
        transform.DOKill();

        transform.DOMoveY(upMovementTarget, delayBeforeHide)
            .SetRelative()
            .SetEase(Ease.OutQuad)
            .OnComplete(()=> {
                fxPopupUI.canvasGroup.DOFade(0, hideSpeed)
                .OnComplete(() => _pool.Release(this));
            });
    }
}
