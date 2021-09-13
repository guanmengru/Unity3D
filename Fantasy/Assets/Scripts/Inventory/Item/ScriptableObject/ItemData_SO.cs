using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Useable,//消耗品
    Weapon,//武器
    Armor//装备
}

[CreateAssetMenu(fileName = "New Item", menuName="Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;//物品类型
    public string itemName;//物品名称
    public Sprite itemIcon;//物品图标
    public int itemAmount;//物品数量
    [TextArea]
    public string description = "";//物品介绍
    public bool stackable;//是否可堆叠

    [Header("Weapon")]
    public GameObject weaponPrefab;
    public AttackData_SO weaponData;



}
