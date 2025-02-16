using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class ObstacleItem : Item
{
    public int health = 1;
    public Sprite damagedSprite; 

    private void Start()
    {
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

    public override void TakeDamage(DamageSource source = DamageSource.Default)
    {
        // stone is only damaged by rockets
        if (itemType == ItemType.Stone && source != DamageSource.Rocket)
        {
            return; // no effect if no rocket
        }

        // if box or vase, apply damage
        health--;

        if (health <= 0)
        {
            // update obstacle count
            BoardManager.Instance.ReduceObstacleCounts(itemType);

            StartCoroutine(DamageEffect());

            // remove from board
            BoardManager.Instance.board[x, y] = null;
        }
        else
        {
            // if vase have 1 health, meaning damaged -> change its sprite to damaged
            if (itemType == ItemType.Vase && health == 1)
            {
                SoundManager.Instance.PlayVaseCrack();
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
}
