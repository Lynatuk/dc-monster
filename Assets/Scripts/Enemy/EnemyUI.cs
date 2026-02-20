using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class EnemyUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public Transform enemy;

    public List<Sprite> monsterSprites;

    [Header("Anim params")]
    [SerializeField] private SpriteRenderer monsterSp;

    [SerializeField] private float deathDuration = 0.3f;
    [SerializeField] private float hitDuration = 0.3f;

    private Material _material;

    public void Setup()
    {
        _material = monsterSp.material;

        SetVisualMonster();
    }

    private void SetVisualMonster()
    {
        monsterSp.sprite = monsterSprites[Random.Range(0, monsterSprites.Count)];

        _material.SetFloat("_HsvShift", Random.Range(0, 361));
        _material.SetFloat("_HsvSaturation", Random.Range(1f, 1.2f));
        _material.SetFloat("_HsvBright", Random.Range(1f, 1.1f));

        monsterSp.transform.localScale = new Vector2(Random.Range(90f, 100f), Random.Range(90f, 100f));

        _material.SetFloat("_FadeAmount", 1f);
        DOTween.To(() => _material.GetFloat("_FadeAmount"), x => _material.SetFloat("_FadeAmount", x), 0, deathDuration);
    }

    public void PlayHit()
    {
        _material.SetFloat("_HitEffectBlend", 0.1f);
        DOTween.To(() => _material.GetFloat("_HitEffectBlend"), x => _material.SetFloat("_HitEffectBlend", x), 0, hitDuration);

        _material.SetFloat("_ChromAberrAmount", 0.25f);
        DOTween.To(() => _material.GetFloat("_ChromAberrAmount"), x => _material.SetFloat("_ChromAberrAmount", x), 0, hitDuration);
    }

    public async UniTask PlayDeath()
    {
        await DOTween.To(() => _material.GetFloat("_FadeAmount"), x => _material.SetFloat("_FadeAmount", x), 1, deathDuration).AsyncWaitForCompletion();
    }
}