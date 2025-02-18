using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

[System.Serializable]
public class LevelData
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public List<string> grid;
}

public class LevelSceneManager : MonoBehaviour
{
    private static LevelSceneManager _instance;
    public static LevelSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("LevelManager");
                _instance = obj.AddComponent<LevelSceneManager>();
                DontDestroyOnLoad(obj);
                _instance.Initialize();
            }
            return _instance;
        }
    }

    private string levelsDirectory = "Levels";
    private Dictionary<int, LevelData> levels = new Dictionary<int, LevelData>();

    public void Initialize()
    {
        LoadLevels();
    }

    private void LoadLevels()
    {
        string path = Path.Combine(Application.streamingAssetsPath, levelsDirectory);

        string[] files = Directory.GetFiles(path, "*.json");
        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            LevelData level = JsonUtility.FromJson<LevelData>(json);

            if (level != null && level.grid != null)
            {
                levels[level.level_number] = level;
            }
        }
    }

    public LevelData GetLevel(int levelNumber)
    {
        if (levels.TryGetValue(levelNumber, out LevelData level))
        {
            return level;
        }
        
        return null;
    }

    public int GetCurrentLevelNumber()
    {
        int currentLevelNumber = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        return currentLevelNumber;
    }

    public void SetCurrentLevelNumber(int levelNumber)
    {
       PlayerPrefs.SetInt("LastPlayedLevel", levelNumber);
       PlayerPrefs.Save();
    }

    public void LoadLevel(int levelNumber)
    {
        int currentLevelNumber = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        BoardManager.Instance.LoadLevelData(currentLevelNumber);
        Debug.Log($"level loaded: {currentLevelNumber}");
    }
}
