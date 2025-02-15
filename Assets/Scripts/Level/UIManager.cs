using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TMP_Text moveText;
    [SerializeField] private GameObject popup;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {   
        tryAgainButton.onClick.AddListener(OnTryAgainClicked);
        exitButton.onClick.AddListener(OnExitClicked);
    }

    public void SetMoveText(int availableMoves)
    {
        if (moveText != null)
        {
            moveText.text = $"{availableMoves}";
        }
    }

    public void ShowPopupPanel() {
        popup.SetActive(true);
        popup.transform.SetAsLastSibling();
    }

    void OnTryAgainClicked()
    {
        int lastPlayedLevel = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        SceneManager.LoadScene("LevelScene");
        BoardManager.Instance.LoadLevelData(lastPlayedLevel);
    }

    void OnExitClicked()
    {
        SceneManager.LoadScene("MainScene");
    }
}
