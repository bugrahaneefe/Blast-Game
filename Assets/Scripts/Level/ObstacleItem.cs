using UnityEngine;

public class ObstacleItem : Item
{
    public int health = 1; // Box = 1, Stone = 1, Vase = 2
    public Sprite damagedSprite; // The sprite when the vase is hit once
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Assign correct health based on obstacle type
        switch (itemType)
        {
            case ItemType.Box:
                health = 1;
                break;
            case ItemType.Stone:
                health = 1;
                break;
            case ItemType.Vase:
                health = 2;
                break;
        }
    }

    public override void TakeDamage()
    {
        health--;

        if (itemType == ItemType.Vase && health == 1)
        {
            ChangeToDamagedSprite();
        }

        if (health <= 0)
        {
            DestroyObstacle();
        }
    }

    private void ChangeToDamagedSprite()
    {
        if (damagedSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = damagedSprite;
        }
    }

    private void DestroyObstacle()
    {
        // Remove from board
        //BoardManager.Instance.RemoveItem(x, y);
        Destroy(gameObject);
    }
}
