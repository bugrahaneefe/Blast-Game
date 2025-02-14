using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public GameObject item;
    public bool isClickable;

    public Node(GameObject _item)
    {
        item = _item;
        isClickable = false;

        Item itemComponent = _item.GetComponent<Item>();
        if (itemComponent == null)
        {
            // If there's no Item component at all, definitely not clickable
            isClickable = false;
            return;
        }

        // If itemType is Stone, Vase, Box, VRocket, or HRocket => not clickable
        switch (itemComponent.itemType)
        {
            case ItemType.Stone:
            case ItemType.Vase:
            case ItemType.Box:
            case ItemType.VRocket:
            case ItemType.HRocket:
                isClickable = false;
                break;

            default:
                // For any other ItemType (including color cubes), it's clickable
                isClickable = true;
                break;
        }
    }
}
