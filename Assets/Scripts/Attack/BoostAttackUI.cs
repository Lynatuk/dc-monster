using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class BoostAttackUI : MonoBehaviour
{
    public Slider slider;

    public Transform lightIcon;

    public TextMeshProUGUI boostValue;

    public ParticleSystem boostEffect;

    public void AnimMin()
    {
        lightIcon.DOScale(0.8f, 0.3f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
