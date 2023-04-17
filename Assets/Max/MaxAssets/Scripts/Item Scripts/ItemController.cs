using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public Item item;

    public void UseItem()
    {
        switch (item.itemType)
        {
            case Item.ItemType.Talisman:
                break;
            case Item.ItemType.Rifle: 
                break;
        }
    }
}
