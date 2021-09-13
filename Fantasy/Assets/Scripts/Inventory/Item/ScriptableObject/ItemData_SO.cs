using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Useable,//����Ʒ
    Weapon,//����
    Armor//װ��
}

[CreateAssetMenu(fileName = "New Item", menuName="Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;//��Ʒ����
    public string itemName;//��Ʒ����
    public Sprite itemIcon;//��Ʒͼ��
    public int itemAmount;//��Ʒ����
    [TextArea]
    public string description = "";//��Ʒ����
    public bool stackable;//�Ƿ�ɶѵ�

    [Header("Weapon")]
    public GameObject weaponPrefab;
    public AttackData_SO weaponData;



}
