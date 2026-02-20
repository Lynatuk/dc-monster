using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public class ToolbarGUI
{
    static bool isLogoChecked = true;

    static ToolbarGUI()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        isLogoChecked = GUILayout.Toggle(isLogoChecked, "Load Logo");

        if (isLogoChecked)
        {
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Logo.unity");
        }
        else
        {
            EditorSceneManager.playModeStartScene = null;
        }
    }
}