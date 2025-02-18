using UnityEngine;
using System.Collections;

public class RocketItem : Item
{
    public GameObject hRightPrefab;
    public GameObject hLeftPrefab;
    public GameObject vUpPrefab;
    public GameObject vDownPrefab;
    public bool ifCombo = false;

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

        // handling of clicked item
        BoardManager.Instance.HandleItemClick(this);
    }

    public override void TakeDamage(DamageSource source = DamageSource.Default)
    {
        SoundManager.Instance.PlayRocketClicked();
        // explode rocket if taken damage, another rocket may also explode rocket
        ExplodeRocket();

        BoardManager.Instance.board[x, y] = null;
        // reset combo for newly generated rockets
        ifCombo = false;
    }

    private void ExplodeRocket()
    {
        BoardManager.Instance.board[x, y] = null;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        spriteRenderer.enabled = false;

        // spawn mini rockets in opposite directions
        Vector3 spawnPosition = transform.position;

        // spawn combo rockets in 3x3 layout
        if (ifCombo) {
            Instantiate(hLeftPrefab, spawnPosition + new Vector3(0f, -1f, 0f) , Quaternion.identity);
            Instantiate(hLeftPrefab, spawnPosition + new Vector3(0f, 1f, 0f) , Quaternion.identity);
            Instantiate(hLeftPrefab, spawnPosition, Quaternion.identity);
            Instantiate(hRightPrefab, spawnPosition + new Vector3(0f, -1f, 0f) , Quaternion.identity);
            Instantiate(hRightPrefab, spawnPosition + new Vector3(0f, 1f, 0f) , Quaternion.identity);
            Instantiate(hRightPrefab, spawnPosition, Quaternion.identity);

            Instantiate(vUpPrefab, spawnPosition + new Vector3(1f, 0f, 0f) , Quaternion.identity);
            Instantiate(vUpPrefab, spawnPosition + new Vector3(-1f, 0f, 0f) , Quaternion.identity);
            Instantiate(vUpPrefab, spawnPosition, Quaternion.identity);
            Instantiate(vDownPrefab, spawnPosition + new Vector3(1f, 0f, 0f) , Quaternion.identity);
            Instantiate(vDownPrefab, spawnPosition + new Vector3(-1f, 0f, 0f) , Quaternion.identity);
            Instantiate(vDownPrefab, spawnPosition, Quaternion.identity);
        }
        else if (isVertical)
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
