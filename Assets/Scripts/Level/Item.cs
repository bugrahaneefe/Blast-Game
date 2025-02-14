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

    // taking damage is common in all item types
    public virtual void TakeDamage()
    {
        Destroy(this.gameObject);
    }
}
