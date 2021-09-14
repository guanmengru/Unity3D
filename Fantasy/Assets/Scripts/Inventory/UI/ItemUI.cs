using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image icon = null;//物品图片
    public Text amount = null;//物品数量
    public InventoryData_SO Bag
    {
        get;
        set;
    }
    public int Index
    {
        get;
        set;
    } = -1;
    public void SetupItemUI(ItemData_SO item,int itemAmount)
    {
        if(item!=null)
        {

            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();
            icon.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
        }

    }
}
