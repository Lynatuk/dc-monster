using UnityEngine;

public class ComboController : MonoBehaviour
{

    public ComboUI comboUI;

    private int currentCombo = 1;

    private int previousComboNumb = -1;

    public void Init()
    {
        comboUI.Setup();
    }

    public void SetupFace(int newFaceIndex, NumericalDiceInfo numericalDiceInfo)
    {
        comboUI.SetupFace(newFaceIndex + 1, numericalDiceInfo);

        if (IsSameHitFace(newFaceIndex))
        {
            currentCombo++;
            AnimateCombo(currentCombo);
        }
        else
        {
            currentCombo = 1;
        }
    }

    private void AnimateCombo(int comboNumb)
    {
        comboUI.SetComboValue(comboNumb);
    }

    private bool IsSameHitFace(int faceIndex)
    {
        bool isSame = previousComboNumb == faceIndex;
        previousComboNumb = faceIndex;

        return isSame;
    }

}

