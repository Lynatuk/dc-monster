using UnityEngine;
using System;
using TMPro;

[Serializable]
public class FxPopupInfo
{
    public AttackInfoType attackInfoType;
    public Sprite icon;

    public TMP_FontAsset font;
    public Color valueColor;
    public Color substrateColor;

    private string value;

    public void SetFxValue(string val) => value = val;

    public string GetFxValue() => value;

}