using UnityEngine;
using UnityEngine.Events;

public class CoinsController : MonoBehaviour
{  
    public static UnityAction updateCoins;

    public CoinsUI coinsUI;

    public bool buttonActive = true;
    public bool plusActive = true;

    void Start()
    {
        updateCoins += coinsUI.UpdateCoinsCount;

        coinsUI.Setup(buttonActive);
        coinsUI.SetCoinsCount();
        coinsUI.SetPlusStateActiv(plusActive);
    }

    private void OnDestroy()
    {
        updateCoins -= coinsUI.SetCoinsCount;
    }
}
