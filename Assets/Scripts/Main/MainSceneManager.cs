using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainSceneManager : MonoBehaviour
{
    public Button levelButton;
    public TMP_Text levelButtonText;
    private int m_currentLevel;
    private int m_maxLevel = 10;

    void Start()
    {
        m_currentLevel = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        
        UpdateLevelButton();
        
        levelButton.onClick.AddListener(OnLevelButtonClicked);
    }

    void UpdateLevelButton()
    {
        if (m_currentLevel <= m_maxLevel)
        {
            levelButtonText.text = "Level " + m_currentLevel.ToString();
        }
        else
        {
            levelButtonText.text = "Finished!";
            levelButton.interactable = false;
        }
    }

    void OnLevelButtonClicked()
    {
        //SoundManager.Instance.PlayClick();
        SceneManager.LoadScene("LevelScene");
    }
}
