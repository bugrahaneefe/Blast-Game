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

    public void SetRocketState(bool state)
    {
        isRocketReady = state;
        spriteRenderer.sprite = state ? rocketState : defaultState;
    }
}
