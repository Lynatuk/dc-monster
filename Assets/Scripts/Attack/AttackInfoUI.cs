using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class AttackInfoUI : MonoBehaviour
{
    public List<AttackPanelInfoUI> panelInfoStyles;

    public int showingDelayMs = 100;

    public GameObject makeThrowTitle;

    public void HidePanels()
    {
        foreach (AttackPanelInfoUI infoUI in panelInfoStyles)
        {
            infoUI.HideAnim();
        }
    }

    public async UniTask SetInfoPanels(List<AttackPanelData> panelInfos)
    {
        makeThrowTitle.SetActive(false);

        for (int i = 0; i < panelInfos.Count; i++)
        {
            panelInfoStyles[i].gameObject.SetActive(true);
            panelInfoStyles[i].SetupStyle(panelInfos[i]);

            await UniTask.Delay(showingDelayMs, cancellationToken: destroyCancellationToken);
        }
    }
}
