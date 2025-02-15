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

        // if item is on rocket state generate rocket on mouse down
        if (isRocketReady)
        {
            // TO DO: spawn rocket at exaclty where cubeitem is
            BoardManager.Instance.HandleItemClick(this);
        }
        else
        {
            // normal handling of clicked item
            BoardManager.Instance.HandleItemClick(this);
        }
    }

    public void SetRocketState(bool state)
    {
        isRocketReady = state;
        spriteRenderer.sprite = state ? rocketState : defaultState;
    }
}
