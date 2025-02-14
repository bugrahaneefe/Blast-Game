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
        // stone is only damaged by rockets
        if (itemType == ItemType.Stone)
        {
            return; // no effect
        }

        // if box or vase, apply damage
        health--;

        if (health <= 0)
        {
            DestroyObstacle();
        }
        else
        {
            // if vase have 1 health, meaning damaged -> change its sprite to damaged
            if (itemType == ItemType.Vase && health == 1)
            {
                ChangeToDamagedSprite();
            }
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
        // remove from board if it still references us
        BoardManager.Instance.board[x, y] = null;
        Destroy(this.gameObject);
    }
}
