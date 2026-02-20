using System.Collections.Generic;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
    [SerializeField] private CoinFlyBehaviour coinPrefab;
    [SerializeField] private int initialSize = 20;

    private readonly Queue<CoinFlyBehaviour> pool = new();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNew();
        }
    }

    private void CreateNew()
    {
        var coin = Instantiate(coinPrefab, transform);
        coin.gameObject.SetActive(false);
        pool.Enqueue(coin);
    }

    public CoinFlyBehaviour Get()
    {
        if (pool.Count == 0)
            CreateNew();

        var coin = pool.Dequeue();
        coin.gameObject.SetActive(true);
        return coin;
    }

    public void Return(CoinFlyBehaviour coin)
    {
        coin.gameObject.SetActive(false);
        pool.Enqueue(coin);
    }
}