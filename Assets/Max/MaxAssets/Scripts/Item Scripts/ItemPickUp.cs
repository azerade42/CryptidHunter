using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;
    private bool inItem;

    public void Pickup()
    {
        InventoryManager.Instance.AddHotbar(item);
        InventoryManager.Instance.AddInventory(item);
        Destroy(gameObject);
        InventoryManager.Instance.ListItems();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inItem = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inItem = false;
        }
    }

    private void Update()
    {
        if (inItem == true) 
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Pickup();
            }
        }
    }
}
