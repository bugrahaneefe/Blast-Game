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

        // guard safe
        if (itemComponent == null) { isClickable = false; return; }

        // stone, vase, box, vrocket, or hrocket are not clickable (hrocket and vrocket for now, to be fixed!)
        switch (itemComponent.itemType)
        {
            case ItemType.Stone:
            case ItemType.Vase:
            case ItemType.Box:
                isClickable = false;
                break;

            default:
                // cubes are clickable
                isClickable = true;
                break;
        }
    }
}
