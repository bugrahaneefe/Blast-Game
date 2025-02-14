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
        // if there are adjacent obstacles to be removed
        HashSet<ObstacleItem> adjacentObstacles = new HashSet<ObstacleItem>();

        foreach (Item item in itemsToRemove)
        {
            if (item == null) continue;

            List<Item> neighbors = GetNeighbors(item);
            foreach (Item neighbor in neighbors)
            {
                ObstacleItem obstacle = neighbor as ObstacleItem;
                if (obstacle != null)
                {
                    adjacentObstacles.Add(obstacle);
                }
            }
        }

        // destroy & remove matched cubes
        foreach (Item i in itemsToRemove)
        {
            if (i == null) continue;

            // remove
            board[i.x, i.y] = null;

            // destroy
            Destroy(i.gameObject);
        }

        // damage adjacent obstacles
        foreach (ObstacleItem obstacle in adjacentObstacles)
        {
            obstacle.TakeDamage(); 
        }

        // start falling logic
        StartCoroutine(FallExistingItems());
    }
    #endregion

    #region falling logic

    // fall Implementation for existing items
    public IEnumerator FallExistingItems()
    {
        // wait for short while before falling
        yield return new WaitForSeconds(0.2f);

        bool hasFallingItems = false;

        float boardWorldHeight = (height - 1) * spacingY;

        for (int x = 0; x < width; x++)
        {
            int writeIndex = height - 1;

            for (int y = height - 1; y >= 0; y--)
            {
                if (board[x, y] != null)
                {
                    if (writeIndex != y)
                    {
                        board[x, writeIndex] = board[x, y];
                        board[x, y] = null;

                        Item item = board[x, writeIndex].item.GetComponent<Item>();
                        float fallDistance = Mathf.Abs(y - writeIndex);
                        item.SetIndicies(x, writeIndex);

                        float finalY = -writeIndex * spacingY + (boardWorldHeight / 2f);

                        // move items down
                        StartCoroutine(MoveItemDown(item, finalY, fallDistance));

                        hasFallingItems = true;
                    }
                    writeIndex--;
                }
            }
        }

        // items landed
        if (hasFallingItems)
        {
            yield return new WaitForSeconds(0.3f);
        }
    }

    // moving items down
    private IEnumerator MoveItemDown(Item item, float targetY, float fallDistance)
    {
        item.isFalling = true;

        float fallSpeed = 4f;
        Vector3 startPos = item.transform.position;
        Vector3 endPos = new Vector3(startPos.x, targetY, startPos.z);

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * fallSpeed;
            item.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime);
            yield return null;
        }
        item.transform.position = endPos;

        yield return StartCoroutine(JumpAnimation(item, fallDistance));

        item.isFalling = false;
    }

    // jump animation
    private IEnumerator JumpAnimation(Item item, float fallDistance)
    {
        if (fallDistance < 1) yield break;

        float jumpHeight = Mathf.Clamp(fallDistance * 0.07f, 0.05f, 0.3f);
        float jumpSpeed = Mathf.Clamp(fallDistance * 0.25f, 1f, 1.8f);

        Vector3 originalPos = item.transform.position;
        Vector3 peakPos = originalPos + new Vector3(0, jumpHeight, 0);

        float upTime = Mathf.Clamp(0.06f * fallDistance, 0.06f, 0.15f);
        float downTime = upTime * 0.6f;

        float elapsedTime = 0f;

        while (elapsedTime < upTime)
        {
            elapsedTime += Time.deltaTime * jumpSpeed;
            item.transform.position = Vector3.Lerp(originalPos, peakPos, elapsedTime / upTime);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < downTime)
        {
            elapsedTime += Time.deltaTime * jumpSpeed;
            item.transform.position = Vector3.Lerp(peakPos, originalPos, elapsedTime / downTime);
            yield return null;
        }

        item.transform.position = originalPos;
    }
    #endregion
}