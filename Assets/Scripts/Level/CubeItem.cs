using UnityEngine;

public class CubeItem : Item
{
    [Header("Cube Sprites")]
    public Sprite defaultState;
    public Sprite rocketState;
    private SpriteRenderer spriteRenderer;
    private bool isRocketReady = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultState;
    }

    private void OnMouseDown()
    {
        // don't click a falling block
        if (isFalling) return;

        Node node = BoardManager.Instance.board[x, y];
        if (node == null || !node.isClickable) return;

        // If this cube is already in rocket state, spawn an actual RocketItem at this position
        if (isRocketReady)
        {
            //SpawnRocketHere();
        }
        else
        {
            // Normal cube: let BoardManager handle matches
            BoardManager.Instance.HandleItemClick(this);
        }
    }

    public void SetRocketState(bool state)
    {
        isRocketReady = state;
        spriteRenderer.sprite = state ? rocketState : defaultState;
    }

    /// <summary>
    /// Spawns an actual rocket at the cube’s position, then removes this cube.
    /// You could do it any way you like – here's an example.
    /// </summary>
    private void SpawnRocketHere()
    {
        // Decide vertical or horizontal rocket prefab (your choice).
        // For simplicity, let's use vertical rocket if it's one of the VRocket types, etc.
        // Or do it randomly:
        bool vertical = (Random.value < 0.5f);
        GameObject rocketPrefab = vertical
            ? BoardManager.Instance.verticalRocketPrefab.gameObject
            : BoardManager.Instance.horizontalRocketPrefab.gameObject;

        Vector3 spawnPos = transform.position;
        // Destroy the cube from the board
        BoardManager.Instance.board[x, y] = null;
        Destroy(gameObject);

        // Instantiate the rocket
        GameObject rocketObj = Instantiate(rocketPrefab, spawnPos, Quaternion.identity);
        rocketObj.transform.SetParent(BoardManager.Instance.transform);

        // If you want your rocket item to know its grid coords, do so:
        RocketItem rocketItem = rocketObj.GetComponent<RocketItem>();
        rocketItem.x = x;
        rocketItem.y = y;

        // Place it in the board array
        //BoardManager.Instance.board[x, y] = rocketItem;
    }
}
