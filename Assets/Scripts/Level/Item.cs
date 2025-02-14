using UnityEngine;

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

public class Item : MonoBehaviour
{
    public ItemType itemType;
    public int x;
    public int y;
    public bool isFalling;

    public void SetIndicies(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    // taking damage is common in all item types
    public virtual void TakeDamage() { }
}
