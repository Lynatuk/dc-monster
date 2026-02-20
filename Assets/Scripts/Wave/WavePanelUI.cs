using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WavePanelUI : MonoBehaviour
{
    public TextMeshProUGUI wavesCountText;

    public List<Image> points;

    public Sprite passPointSp;
    public Sprite activePointSp;
    public Sprite closePointSp;

    public void SetPoint(int index, WavePointState pointState)
    {
        var point = points[index];

        switch (pointState)
        {
            case WavePointState.Passed:
                point.sprite = passPointSp;
                break;
            case WavePointState.InProgress:
                point.sprite = activePointSp;
                break;
            case WavePointState.Closed:
                point.sprite = closePointSp;
                break;
            default:
                point.sprite = closePointSp;
                break;
        }
    }
}
