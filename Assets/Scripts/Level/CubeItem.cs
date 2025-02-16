using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class CubeItem : Item
{
    [Header("Cube Sprites")]
    public Sprite defaultState;
    public Sprite rocketState;
    private bool isRocketReady = false;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer.sprite = defaultState;
    }

    private void OnMouseDown()
    {
        // don't click a falling block
        if (isFalling) return;

        Node node = BoardManager.Instance.board[x, y];
        if (node == null || !node.isClickable) return;

        // handling of clicked item
        BoardManager.Instance.HandleItemClick(this);
    }

    public void SetRocketState(bool state)
    {
        isRocketReady = state;
        spriteRenderer.sprite = state ? rocketState : defaultState;
    }

    public override void TakeDamage() {
        StartCoroutine(DamageEffect());
        // remove from board
        BoardManager.Instance.board[x, y] = null;
    }
}
