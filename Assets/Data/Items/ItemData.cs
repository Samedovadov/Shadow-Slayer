using UnityEngine;

public enum ItemType
{
    Crystal,
    Health
}

[CreateAssetMenu(
    fileName = "ItemData",
    menuName = "Shadow Slayer/Data/Item"
)]
public class ItemData : ScriptableObject
{
    [Header("Item")]
    public string itemName = "New Item";
    public ItemType itemType;

    [Header("Value")]
    public int amount = 1;
}
