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

        // Get currently saved last played level
        int currentLevel = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        GUILayout.Label($"Current Level: {currentLevel}");

        // User Input Field
        newLevelNumber = EditorGUILayout.IntField("New Level Number:", newLevelNumber);

        GUILayout.Space(10);

        // Save Button
        if (GUILayout.Button("Save Level"))
        {
            PlayerPrefs.SetInt("LastPlayedLevel", newLevelNumber);
            PlayerPrefs.Save();
            Debug.Log($"Last played level set to {newLevelNumber}");
            Close();
        }

        // Reset Button
        if (GUILayout.Button("Reset to Default (1)"))
        {
            PlayerPrefs.SetInt("LastPlayedLevel", 1);
            PlayerPrefs.Save();
            Debug.Log("Last played level reset to default (1).");
            Close();
        }
    }
}
