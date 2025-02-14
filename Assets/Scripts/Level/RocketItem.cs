using UnityEngine;
using System.Collections;

public class RocketItem : Item
{
    private bool isVertical; // true = vertical, false = horizontal

    private void Start()
    {
        // assign rocket direction
        isVertical = (itemType == ItemType.VRocket);
    }

    private void OnMouseDown()
    {
        ExplodeRocket();
    }

    public void ExplodeRocket()
    {
        //StartCoroutine(RocketExplosionRoutine());
    }
}
