using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [Header("Inventory Data")]
    public InventoryData_SO InventoryData;
    [Header("Containers")]
    public ContainerUI inventoryUI;

}
