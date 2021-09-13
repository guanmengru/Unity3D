using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    //捡起物品

    public ItemData_SO itemData;
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player")&&Input.GetKeyDown(KeyCode.F))
        {

            //将物品添加进背包
            InventoryManager.Instance.InventoryData.AddItem(itemData, itemData.itemAmount);
            GameManager.Instance.playerStats.EquipWeapon(itemData);
            Destroy(gameObject);
        }
    }
}
