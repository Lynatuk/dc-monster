using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeWindowUI : MonoBehaviour
{
    public Transform facesContainer;
    public FaceUpgradeView faceUpgradePrefab;

    public ProgressSliderController progressSliderController;

    public Button prevDiceButton;
    public Button nextDiceButton;

    public void ClearFaces()
    {
        foreach (Transform child in facesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetNavigationInteractable(bool hasPrev, bool hasNext)
    {
        prevDiceButton.interactable = hasPrev;
        nextDiceButton.interactable = hasNext;
    }

    public void SetOnPrevClick(Action callback)
    {
        prevDiceButton.onClick.RemoveAllListeners();
        prevDiceButton.onClick.AddListener(() => callback.Invoke());
    }

    public void SetOnNextClick(Action callback)
    {
        nextDiceButton.onClick.RemoveAllListeners();
        nextDiceButton.onClick.AddListener(() => callback.Invoke());
    }

    public FaceUpgradeView CreateFacePanel() => Instantiate(faceUpgradePrefab, facesContainer);
}