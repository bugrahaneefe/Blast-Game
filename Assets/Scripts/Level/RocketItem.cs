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
        // TO DO: implement rocket explosion logic here
        Debug.Log("rocket is clicked");
    }
}
