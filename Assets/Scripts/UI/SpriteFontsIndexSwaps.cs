using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;

public class SpriteFontsIndexSwaps : ObjectStateSwapBase
{
    [SerializeField] private List<FontSwapIndex> fontSwaps;
    [SerializeField] private List<FontColorSwapIndex> fontColorSwaps;
    [SerializeField] private List<SpriteSwapIndex> spriteSwaps;

    public override void SetState(int state)
    {
        foreach (var f in fontSwaps)
            f.SetState(state);

        foreach (var s in spriteSwaps)
            s.SetState(state);

        foreach (var s in fontColorSwaps)
            s.SetState(state);
    }
}

[Serializable]
public struct SpriteSwapIndex
{
    [SerializeField] private Image img;
    [SerializeField] private List<Sprite> sprites;

    public void SetState(int state)
    {
        var i = Mathf.Clamp(state, 0, sprites.Count - 1);
        img.sprite = sprites[i];
    }
}

[Serializable]
public struct FontSwapIndex
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private List<Material> fonts;

    public void SetState(int state)
    {
        var i = Mathf.Clamp(state, 0, fonts.Count - 1);
        text.fontMaterial = fonts[i];
    }
}

[Serializable]
public struct FontColorSwapIndex
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private List<Color> fonts;

    public void SetState(int state)
    {
        var i = Mathf.Clamp(state, 0, fonts.Count - 1);
        text.color = fonts[i];
    }
}

[Serializable]
public struct GameObjectSwapIndex
{
    [SerializeField] private List<GameObject> gameObjects;

    public void SetState(int state)
    {
        for (var i = 0; i < gameObjects.Count; i++)
        {
            var go = gameObjects[i];
            if (go)
                go.SetActive(i == state);
        }
    }
}