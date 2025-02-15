using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("move text")]
    [SerializeField] private TMP_Text moveText;

    [Header("popup")]
    [SerializeField] private GameObject popup;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button exitButton;

    [Header("goal_ui")]
    [SerializeField] private Transform goalUIContainer;
    [SerializeField] private GameObject goalBoxPrefab;
    [SerializeField] private GameObject goalStonePrefab;
    [SerializeField] private GameObject goalVasePrefab;
    private Dictionary<ItemType, GameObject> goalPrefabs;
    private Dictionary<ItemType, GameObject> goalUIElements = new Dictionary<ItemType, GameObject>();

    private void Awake()
    {
        Instance = this;

        // initializing goal prefabs dictionary
        goalPrefabs = new Dictionary<ItemType, GameObject>
        {
            { ItemType.Box, goalBoxPrefab },
            { ItemType.Stone, goalStonePrefab },
            { ItemType.Vase, goalVasePrefab }
        };
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
    }

    public void ShowWin() {
        int lastPlayedLevel = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        PlayerPrefs.SetInt("LastPlayedLevel", lastPlayedLevel+1);
        SceneManager.LoadScene("MainScene");
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

    #region Goal UI Setup
    public void SetupGoalUI(int boxCount, int stoneCount, int vaseCount)
    {
        // Clear previous goal UI elements
        foreach (Transform child in goalUIContainer)
        {
            Destroy(child.gameObject);
        }
        goalUIElements.Clear();

        Dictionary<ItemType, int> goalCounts = new Dictionary<ItemType, int>
        {
            { ItemType.Box, boxCount },
            { ItemType.Stone, stoneCount },
            { ItemType.Vase, vaseCount }
        };

        foreach (var goal in goalCounts)
        {
            if (goal.Value > 0 && goalPrefabs.ContainsKey(goal.Key))
            {
                // Instantiate goal prefab inside the UI container
                GameObject goalObject = Instantiate(goalPrefabs[goal.Key], goalUIContainer);
                goalUIElements[goal.Key] = goalObject;

                // Update goal count text if exists
                TMP_Text goalText = goalObject.transform.Find("goalPrefabText_ui")?.GetComponent<TMP_Text>();
                if (goalText != null)
                {
                    goalText.text = goal.Value.ToString();
                }
            }
        }
    }

    public void UpdateGoalCount(ItemType itemType, int newCount)
    {
        if (goalUIElements.ContainsKey(itemType))
        {
            TMP_Text goalText = goalUIElements[itemType].transform.Find("goalPrefabText_ui")?.GetComponent<TMP_Text>();
            if (goalText != null)
            {
                goalText.text = newCount.ToString();
            }
        }
    }
    #endregion
}
