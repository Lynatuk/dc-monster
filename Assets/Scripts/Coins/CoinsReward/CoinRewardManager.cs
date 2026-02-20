using UnityEngine;

public class CoinRewardManager : MonoBehaviour
{
    public static CoinRewardManager Instance { get; private set; }

    [SerializeField] private CoinPool pool;

    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private RectTransform targetUI;

    private void Awake() => Instance = this;

    private bool _isRewardAnimating;

    public void SpawnCoins(Vector3 origin, int rewardAmount)
    {
        _isRewardAnimating = false;
        int count = Mathf.Clamp(rewardAmount / 10, 5, 15);

        for (int i = 0; i < count; i++)
        {
            CoinFlyBehaviour coin = pool.Get();
            coin.LaunchCoin(origin, targetUI, targetCanvas, OnReward, OnCoinCollected);
        }
    }

    private void OnReward()
    {
        if (!_isRewardAnimating)
        {
            _isRewardAnimating = true;
            CoinsController.updateCoins?.Invoke();
        }
    }

    private void OnCoinCollected(CoinFlyBehaviour coin)
    {
        pool.Return(coin);
    }
}
