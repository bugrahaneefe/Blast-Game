using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    public Node[,] board;
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
    [SerializeField] private Sprite boardSprite;
    // spacings
    private float spacingX = 1f;
    private float spacingY = 1.1f;
    private GameObject boardBackground;
    public int width;
    public int height;
    private int availableMoves;

    // obstacle counts
    private int boxCount;
    private int stoneCount;
    private int vaseCount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        int lastPlayedLevel = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        LevelSceneManager.Instance.LoadLevel(lastPlayedLevel);
    }

    public void LoadLevelData(int levelNumber)
    {
        LevelData levelData = LevelSceneManager.Instance.GetLevel(levelNumber);
        if (levelData == null)
        {
            Debug.LogError($"level {levelNumber} data not found.");
            return;
        }

        width = levelData.grid_width;
        height = levelData.grid_height;
        availableMoves = levelData.move_count;

        UIManager.Instance.SetMoveText(availableMoves);

        GenerateBoard(levelData);

        UIManager.Instance.SetupGoalUI(boxCount, stoneCount, vaseCount);
    }

    #region board generation
    private void GenerateBoard(LevelData levelData)
    {
        // initializing board
        board = new Node[width, height];

        boxCount = 0;
        stoneCount = 0;
        vaseCount = 0;

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

                Vector3 spawnPosition = GetPosition(x, y);

                GameObject itemObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                        
                Item item = itemObj.GetComponent<Item>();
                item.itemType = assignedType;
                item.transform.SetParent(this.transform);
                item.transform.localScale = Vector3.one * 0.75f;
                item.SetIndicies(x, y);

                board[x, y] = new Node(itemObj);

                if (assignedType == ItemType.Box)
                {
                    boxCount++;
                }
                else if (assignedType == ItemType.Stone)
                {
                    stoneCount++;
                }
                else if (assignedType == ItemType.Vase)
                {
                    vaseCount++;
                }
            }
        }
        
        CheckForRocketState();
        CenterCamera(levelData);
    }

    private void CheckForRocketState()
    {
        // reset cubeitem state to default
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board[x, y] != null)
                {
                    Item item = board[x, y].item.GetComponent<Item>();
                    if (item is CubeItem cube)
                    {
                        cube.SetRocketState(false);
                    }
                }
            }
        }

        // if 4 or more matched, set cubeitem's state to rocket -> it will lead their sprite to change rocket state
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board[x, y] != null)
                {
                    Item item = board[x, y].item.GetComponent<Item>();
                    if (item is CubeItem)
                    {
                        List<Item> connectedItems = GetConnectedItems(item);

                        if (connectedItems.Count >= 4)
                        {
                            foreach (Item connectedItem in connectedItems)
                            {
                                if (connectedItem is CubeItem cube)
                                {
                                    cube.SetRocketState(true);
                                }
                            }
                        }
                    }
                }
            }
        }
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

    #region camera & background
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
        sr.size = new Vector2(gridWidth + 0.35f, gridHeight + 1.3f);
        sr.sortingOrder = 1;
    }

    private void CenterCamera(LevelData levelData)
    {
        Camera.main.transform.position = new Vector3(0, 2, -10);
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
            availableMoves -= 1;
            UIManager.Instance.SetMoveText(availableMoves);

            // if it is a valid match, remove connected items
            RemoveItems(connectedItems);  

            // if 4 or more matched match clicked, spawn a rocket
            if (connectedItems.Count >= 4)
            {
                SpawnRocket(clickedItem.x, clickedItem.y);
            }

            // if goals are checked, go main menu then next level
            // if else user out of move, show popup panel
            CheckGoalsAndMoves();

            SoundManager.Instance.PlayClick();
        }
    }

    private void SpawnRocket(int x, int y)
    {
        // %50 chance for vertical or horizontal
        bool isVertical = (Random.value > 0.5f);
        GameObject rocketPrefab = isVertical ? verticalRocketPrefab.gameObject : horizontalRocketPrefab.gameObject;

        // generate rocket at x,y
        Vector3 spawnPos = GetPosition(x, y);
        GameObject rocketObj = Instantiate(rocketPrefab, spawnPos, Quaternion.identity);
        rocketObj.transform.SetParent(transform);
        rocketObj.transform.localScale = Vector3.one * 1f;

        // put in board
        board[x, y] = new Node(rocketObj);

        // assign item to rocketitem
        RocketItem rocket = rocketObj.GetComponent<RocketItem>();
        rocket.itemType = isVertical ? ItemType.VRocket : ItemType.HRocket;
        rocket.SetIndicies(x, y);

        // if empty space below
        StartCoroutine(FallExistingItems());
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

        // check up
        if (y + 1 < height && board[x, y + 1]?.item != null)
        {
            neighbors.Add(board[x, y + 1].item.GetComponent<Item>());
        }
        // check down
        if (y - 1 >= 0 && board[x, y - 1]?.item != null)
        {
            neighbors.Add(board[x, y - 1].item.GetComponent<Item>());
        }
        // check left
        if (x - 1 >= 0 && board[x - 1, y]?.item != null)
        {
            neighbors.Add(board[x - 1, y].item.GetComponent<Item>());
        }
        // check right
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

            // remove from board & destroy
            i.TakeDamage();
        }

        // damage adjacent obstacles
        foreach (ObstacleItem obstacle in adjacentObstacles)
        {
            obstacle.TakeDamage(); 
        }

        // start falling logic
        StartCoroutine(FallExistingItems());
    }

    public Vector3 GetPosition(int x, int y)
    {
        float boardWidth = (width - 1) * spacingX;
        float boardHeight = (height - 1) * spacingY;

        float posX = x * spacingX - (boardWidth / 2f);
        float posY = -y * spacingY + (boardHeight / 2f);
        float zPos = 0.001f * y;

        return new Vector3(posX, posY, zPos);
    }

    public void ReduceObstacleCounts(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Box:
                boxCount--;
                break;
            case ItemType.Stone:
                stoneCount--;
                break;
            case ItemType.Vase:
                vaseCount--;
                break;
        }
        UIManager.Instance.UpdateGoalCount(itemType, GetObstacleCount(itemType));
        Debug.Log($"Obstacles left => Box: {boxCount}, Stone: {stoneCount}, Vase: {vaseCount}");
    }

    private int GetObstacleCount(ItemType type)
    {
        return type switch
        {
            ItemType.Box => boxCount,
            ItemType.Stone => stoneCount,
            ItemType.Vase => vaseCount,
            _ => 0
        };
    }
    #endregion

    #region falling logic
    // fall Implementation for existing items
    public IEnumerator FallExistingItems()
    {
        // wait for short while before falling
        yield return new WaitForSeconds(0.1f);

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

        // check if there are new 4 or more matches for rocket state
        CheckForRocketState();
        StartCoroutine(FallNewItems());
    }

    // fall Implementation for new items
    private IEnumerator FallNewItems()
    {
        // wait for short while before falling
        yield return new WaitForSeconds(0.2f);

        bool hasNewItems = false;

        for (int x = 0; x < width; x++)
        {
            int spawnRowCount = 0;

            // from top to bottom
            for (int y = height - 1; y >= 0; y--)
            {
                // if node is null we are going to spawn new random cubeitem for there
                if (board[x, y] == null)
                {
                    hasNewItems = true;

                    var (prefabToSpawn, assignedType) = GetPrefabAndType("rand");

                    int spawnRow = -1 - spawnRowCount;
                    Vector3 spawnPos = GetPosition(x, spawnRow);

                    spawnPos.y += spacingY * 1.5f; 
                    
                    spawnPos.z -= 0.5f;

                    GameObject newItemObj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                    newItemObj.transform.SetParent(this.transform);
                    newItemObj.transform.localScale = Vector3.one * 0.75f;

                    Item newItem = newItemObj.GetComponent<Item>();
                    newItem.itemType = assignedType;

                    newItem.SetIndicies(x, y);
                    board[x, y] = new Node(newItemObj);

                    float finalY = GetPosition(x, y).y;
                    float fallDistance = Mathf.Abs(spawnRow - y);

                    // moving items down
                    StartCoroutine(MoveItemDown(newItem, finalY, fallDistance));

                    spawnRowCount++;
                }
            }
        }

        // wait for landing
        if (hasNewItems)
        {
            yield return new WaitForSeconds(0.2f);
        }

        // check if there are new 4 or more matches for rocket state
        CheckForRocketState();
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

        // jump animation
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

    #region check goals and moves
    public void CheckGoalsAndMoves() {
        if (boxCount <= 0 && stoneCount <= 0 && vaseCount <= 0)
        {
            // all obstacles cleared, win logic
            UIManager.Instance.ShowWin();

        } else if (availableMoves <= 0)
        {
            UIManager.Instance.ShowPopupPanel();
        }
    }
    #endregion
}