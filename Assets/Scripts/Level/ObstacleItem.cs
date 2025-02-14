using UnityEngine;

public class ObstacleItem : Item
{
    public int health = 1;
    public Sprite damagedSprite;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // health of vase is 2, others 1
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

        // if item is vase and it is damaged, turn it into damaged sprite
        if (itemType == ItemType.Vase && health == 1) {
            ChangeToDamagedSprite();
        }

        if (health <= 0) {
            DestroyObstacle();
        }
    }

    private void ChangeToDamagedSprite()
    {
        if (damagedSprite != null && spriteRenderer != null) {
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
