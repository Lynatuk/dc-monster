using DG.Tweening;
using System;
using UnityEngine;

public class CoinFlyBehaviour : MonoBehaviour
{
    [Header("Ground Settings")]
    [SerializeField] private float groundY = -150f;
    [SerializeField] private float groundYRandomness = 0.3f; // Разброс по Y в процентах от groundY

    [Header("Fall Settings")]
    [SerializeField] private float initialFallDuration = 0.3f;
    [SerializeField] private float initialFadeInDuration = 0.1f;

    [Header("Bounce Settings")]
    [SerializeField] private float bounceDuration = 0.2f;
    [SerializeField] private float bounceHorizontalRandomness = 180f;
    [SerializeField] private float bounceVerticalRandomnessMin = 180f;
    [SerializeField] private float bounceVerticalRandomnessMax = 240f;
    [SerializeField] private float bounceEaseOutDurationPercent = 0.6f; // Процент длительности для фазы подъема

    [Header("Fly To UI Settings")]
    [SerializeField] private float flyToUIDuration = 0.5f;
    [SerializeField] private float scaleDownStartDelay = 0f; // Задержка перед уменьшением масштаба
    [SerializeField] private float finalScale = 0.4f;
    [SerializeField] private float fadeOutDelay = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.2f;
    [SerializeField] private float postBounceDelay = 0.2f;

    public void LaunchCoin(Vector3 worldSpawnPosition, RectTransform uiTarget, Canvas canvas, Action onReward, Action<CoinFlyBehaviour> onComplete)
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().DOFade(1, initialFadeInDuration);
        transform.localScale = Vector3.one;

        RectTransform canvasRect = canvas.transform as RectTransform;

        Vector2 spawnScreenPos = Camera.main.WorldToScreenPoint(worldSpawnPosition);
        Vector2 spawnLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, spawnScreenPos, null, out spawnLocalPos);
        transform.localPosition = new Vector3(bounceHorizontalRandomness * UnityEngine.Random.Range(-0.25f, 0.25f), bounceHorizontalRandomness * UnityEngine.Random.Range(-0.25f, 0.25f), 0);

        Vector2 targetScreenPos = RectTransformUtility.WorldToScreenPoint(null, uiTarget.position);
        Vector2 targetLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetScreenPos, null, out targetLocalPos);

        float randomGroundY = groundY * (1f + UnityEngine.Random.Range(-groundYRandomness, groundYRandomness));
        Vector2 groundPos = new Vector2(UnityEngine.Random.Range(-bounceHorizontalRandomness, bounceHorizontalRandomness), randomGroundY);

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLocalMove(groundPos, initialFallDuration).SetEase(Ease.InCubic));

        int sideX = groundPos.x < 0 ? -1 : 1;
        Vector2 jumpPos = groundPos + new Vector2(
            UnityEngine.Random.Range(bounceHorizontalRandomness * sideX / 2f, bounceHorizontalRandomness * sideX / 7f),
            UnityEngine.Random.Range(bounceVerticalRandomnessMin, bounceVerticalRandomnessMax));

        float riseDuration = bounceDuration * bounceEaseOutDurationPercent;
        seq.Append(transform.DOLocalMove(jumpPos, riseDuration).SetEase(Ease.OutQuad));

        float fallDuration = bounceDuration * (1f - bounceEaseOutDurationPercent);
        seq.Append(transform.DOLocalMoveY(randomGroundY, fallDuration).SetEase(Ease.InQuad));

        seq.AppendInterval(postBounceDelay);

        seq.Append(transform.DOLocalMove(targetLocalPos, flyToUIDuration).SetEase(Ease.InSine).OnComplete(() => onReward?.Invoke()));

        seq.Join(transform.DOScale(finalScale, flyToUIDuration).SetDelay(scaleDownStartDelay));

        seq.Join(GetComponent<CanvasGroup>().DOFade(0, fadeOutDuration).SetDelay(fadeOutDelay));

        seq.OnComplete(() => onComplete?.Invoke(this));
    }

}