using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinsUI : MonoBehaviour
{
    public Button bttn;

    public Button plus;

    public TextMeshProUGUI count;

    private readonly float countAnimDuration = 0.3f;

    private int _currentCoins;

    private Tween _coinsTween;

    public void Setup(bool isButtonActive)
    {
        if (isButtonActive)
        {
            bttn.onClick.AddListener(OpenBankCoins);
            plus.onClick.AddListener(OpenBankCoins);
        }
        else
        {
            bttn.enabled = false;
        }
    }

    public void SetCoinsCount()
    {
        _currentCoins = Data.instance.localData.coins;

        count.SetText(Data.instance.localData.coins.ToString("N0"));
    }

    public void UpdateCoinsCount()
    {
        _coinsTween?.Kill();

        int startCoins = _currentCoins;
        _currentCoins = Data.instance.localData.coins;

        _coinsTween = DOVirtual.Int(startCoins, Data.instance.localData.coins, countAnimDuration, (value) =>
        {
            count.SetText(value.ToString("N0"));
        });

    }

    void OpenBankCoins()
    {
        Navigation.ShowWindow(ScenesData.CoinBank);
    }

    public void SetPlusStateActiv(bool plusActive)
    {
        if (!plusActive)
        {
            plus.gameObject.SetActive(false);
        }
        else
        {
            plus.gameObject.SetActive(true);
        }
    }
}
