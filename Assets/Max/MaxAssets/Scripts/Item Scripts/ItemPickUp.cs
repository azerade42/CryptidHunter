using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;

    public void Pickup()
    {
        InventoryManager.Instance.Add(item);
        Destroy(gameObject);
    }
}
