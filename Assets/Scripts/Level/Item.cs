using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public enum ItemType
{
    Red,
    Blue,
    Yellow,
    Green,
    VRocket,
    HRocket,
    Box,
    Stone,
    Vase
}

public enum DamageSource
{
    Default,
    Rocket
}

public class Item : MonoBehaviour
{
    public ItemType itemType;
    public int x;
    public int y;
    public bool isFalling;
    public SpriteRenderer spriteRenderer;
    public ParticleSystem particle;

    protected virtual void Awake()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetIndicies(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    // taking damage is common in all item types but implementation may vary.
    public virtual void TakeDamage(DamageSource source = DamageSource.Default) { }
    
    public IEnumerator DamageEffect() {
        particle.Play();

        if (itemType == ItemType.Box) {
            SoundManager.Instance.PlayBoxBreak();
        }

        if (itemType == ItemType.Stone) {
            SoundManager.Instance.PlayStoneBreak();
        }

        if (itemType == ItemType.Vase) {
            SoundManager.Instance.PlayVaseBreak();
        }

        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(particle.main.startLifetime.constantMax);

        // destroy gameobject
        Destroy(this.gameObject);
    }
}
