using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SafeAreaFit))]
public class SafeAreaFitEditor : Editor
{
    SafeAreaFit inspector;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        inspector = (SafeAreaFit)target;

        if (GUILayout.Button("Safe Area"))
            SetArea(true, false);
        if (GUILayout.Button("Centered Safe Area"))
            SetArea(true, true);
        if (GUILayout.Button("Undo Safe Area Preview"))
            SetArea(false);
    }

    private void SetArea(bool _isSafeArea, bool _isCentered = false)
    {
        inspector.GetComponent<SafeAreaFit>().DebugSafeArea(_isSafeArea, _isCentered);
    }
}
