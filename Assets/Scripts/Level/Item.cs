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

    // Virtual method for taking damage (obstacles might override this)
    public virtual void TakeDamage()
    {
        // Cubes might just get destroyed on blast
        Destroy(this.gameObject);
    }
}
