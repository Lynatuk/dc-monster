using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FxPopupUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public Image icon;
    public Image substrate;

    public TextMeshProUGUI valueText;

    public void Setup(FxPopupInfo fxPopupInfo)
    {
        icon.sprite = fxPopupInfo.icon;

        valueText.font = fxPopupInfo.font;
        valueText.SetText(fxPopupInfo.GetFxValue());

        valueText.color = fxPopupInfo.valueColor;
        substrate.color = fxPopupInfo.substrateColor;
    }

}
