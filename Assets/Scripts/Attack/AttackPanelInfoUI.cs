using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AttackPanelInfoUI : MonoBehaviour
{
    public Image panel;
    public Image icon;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI valueText;

    public float showingSpeed = 0.2f;

    public void SetupStyle(AttackPanelData attackPanelData)
    {
        transform.localScale = Vector3.zero;

        icon.sprite = attackPanelData.icon;
        panel.color = attackPanelData.colorPanel;

        titleText.SetText(attackPanelData.name);
        valueText.SetText(attackPanelData.value);

        ShowAnim();
    }

    private void ShowAnim()
    {
        transform.DOScale(Vector3.one, showingSpeed);
    }

    public void HideAnim()
    {
        gameObject.SetActive(false);
    }
}