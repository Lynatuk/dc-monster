using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Dice/DiceData")]
public class DiceData : ScriptableObject
{
    public List<NumericalDiceInfo> numericalInfoDices;

    public DiceProgress GetCurrentDiceProgress()
    {
        DiceType diceType = Data.instance.localData.currentNumericalDice;

        if (Data.instance.localData.dicesProgress.TryGetValue(diceType, out var progress))
            return progress;

        return Data.instance.localData.GetOrCreateProgress(diceType);
    }

    public NumericalDiceInfo GetNumericalDiceInfo(DiceFaceRarity diceFaceRarity) => numericalInfoDices.Find(info => info.diceFaceRarity == diceFaceRarity);
}

[Serializable]
public class NumericalDiceInfo
{
    public DiceFaceRarity diceFaceRarity;

    public Sprite faceSprite;
    public Material faceMat;
    public Color numbColor;
    public Material numbMat;
}

[Serializable]
public class DiceVisualInfo
{
    public int countFaces;

    public List<Image> faces;
    public List<TextMeshProUGUI> faceNumb;
    public List<ParticleSystem> particles;

    public Quaternion[] faceRotations = new Quaternion[]
    {
            Quaternion.Euler(0, 180, 0),        // 1
            Quaternion.Euler(270, 90, 0),       // 2
            Quaternion.Euler(270, 180, 0),      // 3
            Quaternion.Euler(90, 180, 0),       // 4
            Quaternion.Euler(270, 270, 0),      // 5
            Quaternion.Euler(0, 0, 180),        // 6
    };

    public void EnableParticles()
    {
        foreach (var item in particles)
        {
            item.Play();
        }
    }

    public void SetupParticlesLifetime(float maxLifetime)
    {
        foreach (var item in particles)
        {
            var main = item.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, maxLifetime-0.1f);
        }
    }

    public void DisableParticles()
    {
        foreach (var item in particles)
        {
            item.Stop();
        }
    }

    public void SetFace(int faceId, Sprite faceImage, Material mat)
    {
        faces[faceId].sprite = faceImage;
        faces[faceId].material = mat;
    }

    public void SetFaceNumb(int faceId, NumericalDiceInfo info)
    {
        faceNumb[faceId].SetText("{0}", faceId + 1);
        faceNumb[faceId].color = info.numbColor;
        faceNumb[faceId].fontSharedMaterial = info.numbMat;
    }

    public void SetNumb(int faceId, int number)
    {
        faceNumb[faceId].SetText("{0}", number);
    }
}

[Serializable]
public class DiceProgress
{
    public Dictionary<int, FaceProgress> faceProgresses = new();

    public FaceProgress GetFaceProgress(int faceIndex) => faceProgresses[faceIndex];
}

[Serializable]
public class FaceProgress
{
    public DiceFaceRarity diceFaceLevel = DiceFaceRarity.Wooden;
    public float damageMultiplier = 1;
}