
using UnityEngine;

public class WavePanelController : MonoBehaviour
{
    public WavePanelUI wavePanelUI;

    public void Init(int wave, int stage)
    {
        SetWaveNumber(wave, stage);
    }

    public void SetWaveNumber(int wave, int stage)
    {
        wavePanelUI.SetPoint(stage, WavePointState.InProgress);

        for (int i = 0; i < stage; i++)
        {
            wavePanelUI.SetPoint(i, WavePointState.Passed);
        }

        for (int i = stage + 1; i < wavePanelUI.points.Count; i++)
        {
            wavePanelUI.SetPoint(i, WavePointState.Closed);
        }

        wavePanelUI.wavesCountText.SetText("Волна: {0}", wave + 1);
    }

    public void UpdateStage(int stage)
    {
        if (stage > 0)
        {
            wavePanelUI.SetPoint(stage - 1, WavePointState.Passed);
            wavePanelUI.SetPoint(stage, WavePointState.InProgress);
        }
    }

}
