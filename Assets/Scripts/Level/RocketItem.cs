using UnityEngine;
using System.Collections;

public class RocketItem : Item
{
    private bool isVertical; // true = vertical, false = horizontal

    private void Start()
    {
        // Randomly assign rocket direction when created
        if (itemType == ItemType.VRocket)
        {
            isVertical = true;
        }
        else
        {
            isVertical = false;
        }
    }

    private void OnMouseDown()
    {
        ExplodeRocket();
    }

    public void ExplodeRocket()
    {
        StartCoroutine(RocketExplosionRoutine());
    }

    private IEnumerator RocketExplosionRoutine()
    {
        // Visual effect for explosion (you can add a particle effect)
        yield return new WaitForSeconds(0.2f);

        /*// Damage adjacent obstacles
        BoardManager.Instance.DamageObstacles(this);

        // Spawn two moving rockets in opposite directions
        if (isVertical)
        {
            BoardManager.Instance.SpawnRocket(x, y, Vector2.up);
            BoardManager.Instance.SpawnRocket(x, y, Vector2.down);
        }
        else
        {
            BoardManager.Instance.SpawnRocket(x, y, Vector2.left);
            BoardManager.Instance.SpawnRocket(x, y, Vector2.right);
        }

        // Remove rocket from board
        BoardManager.Instance.RemoveItem(x, y);*/
        Destroy(gameObject);
    }
}
