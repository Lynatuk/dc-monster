using UnityEngine;
using TMPro;
using DG.Tweening;

public class WaveUI : MonoBehaviour
{
    [Header("BossTimePanel")]
    public Animator bossTimeAnimator;
    public ParticleSystem glowBossPanelEffect;

    [Header("BossTimer")]
    [SerializeField] private Transform timerTransform;
    [SerializeField] private TextMeshProUGUI timerText;

    private Tween _timerScaleTween;
    private float _currentTimerValue;
    private bool _isTimerActive;

    private const float BOSS_TIMER_DURATION = 30f;
    private const float ANIMATION_DURATION = 0.3f;
    private const float SCALE_OVERSHOOT = 1.2f;

    public void Setup(int wave, int stage)
    {

    }

    public void ShowBossPanel()
    {
        bossTimeAnimator.enabled = true;
        bossTimeAnimator.SetTrigger("Show");
        glowBossPanelEffect.Play();
    }

    public void ShowBossTimer()
    {
        if (timerTransform == null || timerText == null)
        {
            Debug.LogWarning("Timer Transform or TextMeshPro not assigned in WaveUI!");
            return;
        }

        _isTimerActive = true;
        _currentTimerValue = BOSS_TIMER_DURATION;
        
        timerTransform.localScale = Vector3.zero;
        timerText.gameObject.SetActive(true);
        
        _timerScaleTween?.Kill();
        _timerScaleTween = timerTransform.DOScale(Vector3.one * SCALE_OVERSHOOT, ANIMATION_DURATION)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                timerTransform.DOScale(Vector3.one, ANIMATION_DURATION * 0.5f)
                    .SetEase(Ease.InBack);
            });

        UpdateTimerText();
    }

    public void HideBossTimer()
    {
        if (timerTransform == null || timerText == null)
            return;

        _isTimerActive = false;
        _timerScaleTween?.Kill();
        
        _timerScaleTween = timerTransform.DOScale(Vector3.one * SCALE_OVERSHOOT, ANIMATION_DURATION * 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                timerTransform.DOScale(Vector3.zero, ANIMATION_DURATION)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        timerText.gameObject.SetActive(false);
                    });
            });
    }

    public void UpdateTimer(float remainingTime)
    {
        if (!_isTimerActive)
            return;

        _currentTimerValue = remainingTime;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int totalSeconds = Mathf.CeilToInt(_currentTimerValue);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            timerText.text = $"{minutes:D2}:{seconds:D2}";
        }
    }

    public float GetBossTimerDuration()
    {
        return BOSS_TIMER_DURATION;
    }

    private void OnDestroy()
    {
        _timerScaleTween?.Kill();
    }
}
