using UnityEngine;
using System.Collections;

public class RocketItem : Item
{
    public GameObject hRightPrefab;
    public GameObject hLeftPrefab;
    public GameObject vUpPrefab;
    public GameObject vDownPrefab;

    private bool isVertical; // true = vertical, false = horizontal

    private void Start()
    {
        // assign rocket direction
        isVertical = (itemType == ItemType.VRocket);
    }

    private void OnMouseDown()
    {
        // don't explode while falling
        if (isFalling) return; 

        ExplodeRocket();
        BoardManager.Instance.board[x, y] = null;
    }

    public override void TakeDamage(DamageSource source = DamageSource.Default)
    {
        // another rocket may also explode rocket
        ExplodeRocket();

        BoardManager.Instance.board[x, y] = null;
    }

    private void ExplodeRocket()
    {
        BoardManager.Instance.board[x, y] = null;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        spriteRenderer.enabled = false;

        // spawn mini rockets in opposite directions
        Vector3 spawnPosition = transform.position;
        if (isVertical)
        {
            // spawn mini rocket to up
            Instantiate(vUpPrefab, spawnPosition + Vector3.up, Quaternion.identity);

            // spawn mini rocket to down
            Instantiate(vDownPrefab, spawnPosition + Vector3.down, Quaternion.identity);
        }
        else
        {
            // spawn mini rocket to right
            Instantiate(hRightPrefab, spawnPosition + Vector3.right, Quaternion.identity);

            // spawn mini rocket to left
            Instantiate(hLeftPrefab, spawnPosition + Vector3.left, Quaternion.identity);
        }

        // destroy current main rocket
        StartCoroutine(DamageEffect());
    }
}
