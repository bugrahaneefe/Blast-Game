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

    /// <summary>
    /// Called whenever the obstacle takes damage (blast or rocket).
    /// For Stone, we only call this from rocket. 
    /// For Box/Vase, can be from adjacency or rocket.
    /// </summary>
    public override void TakeDamage()
    {
        // Stone? If it's from a blast, we do nothing. But BoardManager won't call TakeDamage() for Stone blasts anyway.
        // So if we're here, that means it's definitely from a rocket, so it applies.

        health--;

        // If Vase with health=1, switch to damaged sprite
        if (itemType == ItemType.Vase && health == 1)
        {
            ChangeToDamagedSprite();
        }

        if (health <= 0)
        {
            DestroyObstacle();
        }
    }

    // Inside ObstacleItem:
    public void TakeBlastDamage()
    {
        // If Stone, do nothing, because Stone is only damaged by rockets
        if (itemType == ItemType.Stone)
        {
            return; // no effect
        }

        // If Box or Vase, apply damage
        // For a vase, reduce health by 1
        health--;

        if (health <= 0)
        {
            DestroyObstacle();
        }
        else
        {
            // If vase has more health, switch to damaged sprite if needed
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
        if (BoardManager.Instance.board[x, y] != null && BoardManager.Instance.board[x, y].item == this)
        {
            BoardManager.Instance.board[x, y] = null;
        }
        Destroy(this.gameObject);
    }
}
