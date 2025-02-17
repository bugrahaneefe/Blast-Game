using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

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

    [Header("celebration")]
    [SerializeField] private GameObject celebrationPopup;

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
        StartCoroutine(ShowCelebrationEffect());
    }

    private IEnumerator ShowCelebrationEffect()
    {
        // show celebration panel
        celebrationPopup.SetActive(true);

        // wait for 3 sec
        yield return new WaitForSeconds(3f);
        LevelSceneManager.Instance.SetCurrentLevelNumber(LevelSceneManager.Instance.GetCurrentLevelNumber() + 1);
                Debug.Log($"it is called {LevelSceneManager.Instance.GetCurrentLevelNumber()}");
        SceneManager.LoadScene("MainScene");

        // hide celebration panel
        celebrationPopup.SetActive(false);
    }

    void OnTryAgainClicked()
    {
        SceneManager.LoadScene("LevelScene");
        BoardManager.Instance.LoadLevelData(LevelSceneManager.Instance.GetCurrentLevelNumber());
    }

    void OnExitClicked()
    {
        SceneManager.LoadScene("MainScene");
    }

    #region Goal UI Setup
    public void SetupGoalUI(int boxCount, int stoneCount, int vaseCount)
    {
        // reset goal panel
        foreach (Transform child in goalUIContainer)
        {
            Destroy(child.gameObject);
        }
        goalUIElements.Clear();

        // assign item types in dictionary with counts (derived from board manager)
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
                // generate goal ui prefabs
                GameObject goalObject = Instantiate(goalPrefabs[goal.Key], goalUIContainer);
                goalUIElements[goal.Key] = goalObject;

                // update simultaneously goal count text
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
            Image checkmark = goalUIElements[itemType].transform.Find("checkmark_ui")?.GetComponent<Image>();
            if (goalText != null)
            {
                if (newCount == 0) {
                    goalText.gameObject.SetActive(false);
                    checkmark.gameObject.SetActive(true);
                } else {
                    goalText.text = newCount.ToString();
                }
            }
        }
    }
    #endregion
}
