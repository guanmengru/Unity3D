using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType
{
    Bag,
    Weapon,
    Armor
}
public class SlotHolder : MonoBehaviour
{
    public SlotType slotType;
    public ItemUI ItemUI;
    public void UpdateItem()
    {
        switch(slotType)
        {
            case SlotType.Bag:
                ItemUI.Bag = InventoryManager.Instance.InventoryData;
                break;

            case SlotType.Weapon:
                break;

            case SlotType.Armor:
                break;
        }
        var item = ItemUI.Bag.items[ItemUI.Index];
        ItemUI.SetupItemUI(item.itemData, item.amount);
    }
}
