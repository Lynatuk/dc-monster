using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GemsUI : MonoBehaviour
{
    public Button bttn;

    public Button plus;

    public TextMeshProUGUI count;

    private float _countAnimDuration = 0.3f;

    private int _currentCoins;

    private Tween _gemsTween;

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

    public void SetGemsCount()
    {
        _currentCoins = Data.Instance.localData.gems;

        count.SetText(Data.Instance.localData.gems.ToString("N0"));
    }

    public void UpdateGemsCount()
    {
        _gemsTween?.Kill();

        int startCoins = _currentCoins;
        _currentCoins = Data.Instance.localData.gems;

        _gemsTween = DOVirtual.Int(startCoins, Data.Instance.localData.gems, _countAnimDuration, (value) =>
        {
            count.SetText(value.ToString("N0"));
        });

    }

    private void OpenBankCoins()
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
