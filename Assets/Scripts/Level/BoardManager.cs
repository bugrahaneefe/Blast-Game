using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Item Prefabs")]
    public Item redItemPrefab;
    public Item blueItemPrefab;
    public Item yellowItemPrefab;
    public Item greenItemPrefab;
    public Item verticalRocketPrefab;
    public Item horizontalRocketPrefab;
    public Item boxPrefab;
    public Item stonePrefab;
    public Item vasePrefab;

    private float spacingX = 1f;
    private float spacingY = 1.1f;

    [Header("Board Background")]
    [SerializeField] private Sprite boardSprite;
    private GameObject boardBackground;
    private Item[,] board;

    private void Awake()
    {
        Instance = this;
    }

    private void Start() 
    {
        LevelSceneManager.Instance.LoadLevel(3);
    }

    public void LoadLevelData(int levelNumber)
    {
        LevelData levelData = LevelSceneManager.Instance.GetLevel(levelNumber);
        if (levelData == null)
        {
            Debug.LogError($"Level {levelNumber} data not found.");
            return;
        }
        Debug.Log("level created");

        GenerateBoard(levelData);
    }

    // generate board grid based on data of level
    private void GenerateBoard(LevelData levelData)
    {
        board = new Item[levelData.grid_width, levelData.grid_height];

        float boardWidth = (levelData.grid_width - 1) * spacingX;
        float boardHeight = (levelData.grid_height - 1) * spacingY;

        // set board background
        UpdateBoardBackground(levelData.grid_width, levelData.grid_height);

        // generate board items manually
        for (int y = 0; y < levelData.grid_height; y++)
        {
            for (int x = 0; x < levelData.grid_width; x++)
            {
                string itemType = levelData.grid[y * levelData.grid_width + x];
                GameObject prefabToSpawn = GetPrefabForItemType(itemType);
                if (prefabToSpawn == null) continue;

                float posX = x * spacingX - (boardWidth / 2f);
                float posY = -y * spacingY + (boardHeight / 2f);
                float zPosition = 0.001f * y;
                Vector3 spawnPosition = new Vector3(posX, posY, zPosition);

                Item newItem = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity).GetComponent<Item>();
                newItem.transform.SetParent(this.transform);
                newItem.transform.localScale = Vector3.one * 0.75f;

                newItem.x = x;
                newItem.y = y;
                newItem.itemType = ParseItemType(itemType);

                board[x, y] = newItem;
            }
        }

        // adjust camera
        CenterCamera(levelData);
    }

    // Set board background
    private void UpdateBoardBackground(int gridWidth, int gridHeight)
    {
        if (boardBackground == null)
        {
            boardBackground = new GameObject("BoardBackground");
            boardBackground.transform.SetParent(this.transform);
            boardBackground.AddComponent<SpriteRenderer>().sprite = boardSprite;
        }

        SpriteRenderer sr = boardBackground.GetComponent<SpriteRenderer>();

        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = new Vector2(gridWidth + 0.4f, gridHeight + 1.2f);
        sr.sortingOrder = 1;
    }

    // Set valid camera position
    private void CenterCamera(LevelData levelData)
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        float ortSize = Mathf.Max(levelData.grid_height, levelData.grid_width);
        float zoomFactor = Mathf.Lerp(1.13f, 1.1f, (ortSize - 4) / 8f);
        Camera.main.orthographicSize = ortSize * zoomFactor;
    }

    private GameObject GetPrefabForItemType(string itemType)
    {
        switch (itemType)
        {
            case "r": return redItemPrefab.gameObject;
            case "b": return blueItemPrefab.gameObject;
            case "y": return yellowItemPrefab.gameObject;
            case "g": return greenItemPrefab.gameObject;
            case "vro": return verticalRocketPrefab.gameObject;
            case "hro": return horizontalRocketPrefab.gameObject;
            case "bo": return boxPrefab.gameObject;
            case "s": return stonePrefab.gameObject;
            case "v": return vasePrefab.gameObject;
            case "rand": return GetRandomCubePrefab();
            default: return null;
        }
    }

    private GameObject GetRandomCubePrefab()
    {
        Item[] randomCubes = { redItemPrefab, blueItemPrefab, yellowItemPrefab, greenItemPrefab };
        return randomCubes[Random.Range(0, randomCubes.Length)].gameObject;
    }

    private ItemType ParseItemType(string itemType)
    {
        return itemType switch
        {
            "r" => ItemType.Red,
            "b" => ItemType.Blue,
            "y" => ItemType.Yellow,
            "g" => ItemType.Green,
            "vro" => ItemType.VRocket,
            "hro" => ItemType.HRocket,
            "bo" => ItemType.Box,
            "s" => ItemType.Stone,
            "v" => ItemType.Vase,
            "rand" => (ItemType)Random.Range(0, 4),
            _ => ItemType.Red
        };
    }
}
