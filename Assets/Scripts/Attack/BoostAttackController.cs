using UnityEngine;
using DG.Tweening;

public class BoostAttackController : MonoBehaviour
{
    public BoostAttackUI boostAttackUI;

    [SerializeField] private Gradient boostGradient;

    public void SetBoost(float boostValue)
    {
        boostAttackUI.slider.DOValue(boostValue, 0.3f);
        boostAttackUI.boostValue.transform.DOScale(1f + boostValue / 10, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        boostAttackUI.boostValue.SetText("x{0}", boostValue);

        float t = Mathf.InverseLerp(1f, 3f, boostValue);
        Color targetColor = boostGradient.Evaluate(t);
        boostAttackUI.boostValue.DOColor(targetColor, 0.3f);

        if (boostValue > 1)
        {
            boostAttackUI.boostEffect.Play();
        }
    }
}
