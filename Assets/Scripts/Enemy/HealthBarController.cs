using DG.Tweening;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public HealthBarUI healthBarUI;

    private Tween _healthCountTween;

    private double _currentHealthCount;

    public void Setup(double currentHealth)
    {
        healthBarUI.redSlider.value = 1;
        healthBarUI.whiteSlider.value = 1;

        healthBarUI.healthCount.SetText(BigNumberFormatter.Format(currentHealth));
    }

    public void UpdateHealth(float newTargetValue)
    {
        healthBarUI.redSlider.value = newTargetValue;

        healthBarUI.whiteSlider.DOValue(newTargetValue - 0.01f, 0.3f).SetEase(Ease.OutCubic).SetDelay(0.2f);
    }

    public void UpdateHealthCount(double currentHealth)
    {
        _healthCountTween?.Kill();

        double startHealthCount = _currentHealthCount;
        _currentHealthCount = currentHealth;

        _healthCountTween = DOVirtual.Float(0f, 1f, 0.3f, t =>
        {
            double value = startHealthCount + (_currentHealthCount - startHealthCount) * t;
            healthBarUI.healthCount.SetText(BigNumberFormatter.Format(value));
        });
    }
}
