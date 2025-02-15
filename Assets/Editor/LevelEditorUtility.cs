using UnityEditor;
using UnityEngine;

public class LevelEditorUtility : EditorWindow
{
    private static int newLevelNumber = 1;

    [MenuItem("Game Settings/Adjust Game Level")]
    private static void ShowWindow()
    {
        LevelEditorUtility window = GetWindow<LevelEditorUtility>("Adjust Game Level");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Adjust Game Level", EditorStyles.boldLabel);

        // current level
        int currentLevel = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        GUILayout.Label($"Current Level: {currentLevel}");

        // input for new level number
        newLevelNumber = EditorGUILayout.IntField("New Level Number:", newLevelNumber);

        GUILayout.Space(10);

        // save button
        if (GUILayout.Button("Save Level"))
        {
            PlayerPrefs.SetInt("LastPlayedLevel", newLevelNumber);
            PlayerPrefs.Save();
            Close();
        }

        // reset to first level
        if (GUILayout.Button("Reset to First Level"))
        {
            PlayerPrefs.SetInt("LastPlayedLevel", 1);
            PlayerPrefs.Save();
            Close();
        }
    }
}
