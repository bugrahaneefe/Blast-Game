using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    public Node[,] board;
    private int width;
    private int height;

    private void Awake()
    {
        Instance = this;
    }

    private void Start() 
    {
        // load level
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

        width = levelData.grid_width;
        height = levelData.grid_height;
        Debug.Log("level created");

        GenerateBoard(levelData);
    }

    #region Board Generation
    private void GenerateBoard(LevelData levelData)
    {
        // initializing board
        board = new Node[width, height];

        float boardWidth = (width - 1) * spacingX;
        float boardHeight = (height - 1) * spacingY;

        // set background 
        UpdateBoardBackground(width, height);

        // items generated based on level information
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                string type = levelData.grid[y * width + x];

                var (prefabToSpawn, assignedType) = GetPrefabAndType(type);
                if (prefabToSpawn == null) continue;

                float posX = x * spacingX - (boardWidth / 2f);
                float posY = -y * spacingY + (boardHeight / 2f);
                float zPosition = 0.001f * y;
                Vector3 spawnPosition = new Vector3(posX, posY, zPosition);

                GameObject itemObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                        
                Item item = itemObj.GetComponent<Item>();
                item.itemType = assignedType;
                item.transform.SetParent(this.transform);
                item.transform.localScale = Vector3.one * 0.75f;
                item.SetIndicies(x, y);

                Debug.Log($"Generated {type} -> {item.itemType} at ({x}, {y})");

                board[x, y] = new Node(itemObj);
            }
        }

        CenterCamera(levelData);
    }

    private (GameObject, ItemType) GetPrefabAndType(string itemType)
    {
        if (itemType == "rand")
        {
            (Item prefab, ItemType type)[] randomCubes =
            {
                (redItemPrefab, ItemType.Red),
                (blueItemPrefab, ItemType.Blue),
                (yellowItemPrefab, ItemType.Yellow),
                (greenItemPrefab, ItemType.Green)
            };

            var selected = randomCubes[Random.Range(0, randomCubes.Length)];
            return (selected.prefab.gameObject, selected.type);
        }

        GameObject prefab = itemType switch
        {
            "r"   => redItemPrefab.gameObject,
            "b"   => blueItemPrefab.gameObject,
            "y"   => yellowItemPrefab.gameObject,
            "g"   => greenItemPrefab.gameObject,
            "vro" => verticalRocketPrefab.gameObject,
            "hro" => horizontalRocketPrefab.gameObject,
            "bo"  => boxPrefab.gameObject,
            "s"   => stonePrefab.gameObject,
            "v"   => vasePrefab.gameObject,
            _     => null
        };

        ItemType parsedType = itemType switch
        {
            "r"   => ItemType.Red,
            "b"   => ItemType.Blue,
            "y"   => ItemType.Yellow,
            "g"   => ItemType.Green,
            "vro" => ItemType.VRocket,
            "hro" => ItemType.HRocket,
            "bo"  => ItemType.Box,
            "s"   => ItemType.Stone,
            "v"   => ItemType.Vase,
            _     => ItemType.Red
        };

        return (prefab, parsedType);
    }
    #endregion

    #region Camera & Background
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

    private void CenterCamera(LevelData levelData)
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
        float ortSize = Mathf.Max(levelData.grid_height, levelData.grid_width);
        float zoomFactor = Mathf.Lerp(1.13f, 1.1f, (ortSize - 4) / 8f);
        Camera.main.orthographicSize = ortSize * zoomFactor;
    }
    #endregion

    #region removing logic
    public void HandleItemClick(Item clickedItem)
    {
        if (clickedItem == null) return;

        List<Item> connectedItems = GetConnectedItems(clickedItem);

        if (connectedItems.Count >= 2)
        {
            RemoveItems(connectedItems);
        }
    }

    // each matched pairs should be unique & stored in list of blocks
    public List<Item> GetConnectedItems(Item startItem)
    {
        List<Item> result = new List<Item>();
        Queue<Item> queue = new Queue<Item>();
        HashSet<Item> visited = new HashSet<Item>();

        queue.Enqueue(startItem);
        visited.Add(startItem);

        while (queue.Count > 0)
        {
            Item current = queue.Dequeue();
            result.Add(current);

            foreach (Item neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && neighbor.itemType == startItem.itemType)
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return result;
    }

    // determine matches
    public List<Item> GetNeighbors(Item item)
    {
        List<Item> neighbors = new List<Item>();

        int x = item.x;
        int y = item.y;

        // Check up
        if (y + 1 < height && board[x, y + 1]?.item != null)
        {
            neighbors.Add(board[x, y + 1].item.GetComponent<Item>());
        }
        // Check down
        if (y - 1 >= 0 && board[x, y - 1]?.item != null)
        {
            neighbors.Add(board[x, y - 1].item.GetComponent<Item>());
        }
        // Check left
        if (x - 1 >= 0 && board[x - 1, y]?.item != null)
        {
            neighbors.Add(board[x - 1, y].item.GetComponent<Item>());
        }
        // Check right
        if (x + 1 < width && board[x + 1, y]?.item != null)
        {
            neighbors.Add(board[x + 1, y].item.GetComponent<Item>());
        }

        return neighbors;
    }

    // remove items & apply fall implementations
    public void RemoveItems(List<Item> itemsToRemove)
{
    // 1) Collect all obstacles adjacent to these cubes
    HashSet<ObstacleItem> adjacentObstacles = new HashSet<ObstacleItem>();

    // For each cube being removed, look at neighbors
    foreach (Item item in itemsToRemove)
    {
        if (item == null) continue;

        // Find neighbors of this item
        List<Item> neighbors = GetNeighbors(item);
        foreach (Item neighbor in neighbors)
        {
            // Check if neighbor is an ObstacleItem
            ObstacleItem obstacle = neighbor as ObstacleItem;
            if (obstacle != null)
            {
                // It's an obstacle, but we only add if it can be damaged by blasts:
                // (Stone won't take blast damage, but let's add it for completeness;
                //  the obstacle code itself can ignore if it's stone.)
                adjacentObstacles.Add(obstacle);
            }
        }
    }

    // 2) Destroy the matched cubes
    foreach (Item i in itemsToRemove)
    {
        if (i == null) continue;

        // Remove from board
        board[i.x, i.y] = null;

        // Destroy object
        Destroy(i.gameObject);
    }

    // 3) Damage obstacles (boxes/vases) exactly once each
    //    Stone items won't be affected, since they ignore blast damage.
    foreach (ObstacleItem obstacle in adjacentObstacles)
    {
        obstacle.TakeBlastDamage(); 
    }

    // Optionally, start falling logic
    //StartCoroutine(FallExistingItems());
}

    #endregion
}