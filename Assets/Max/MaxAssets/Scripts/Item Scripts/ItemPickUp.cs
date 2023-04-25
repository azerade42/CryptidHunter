using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;

    public GameObject playerObj;

    public void Pickup()
    {
        item.playerObj = this.playerObj;
        InventoryManager.Instance.AddHotbar(item);
        InventoryManager.Instance.AddInventory(item);
        InventoryManager.Instance.ListItems();
        InventoryManager.Instance.equippedItem = item;

        if (item.id < 3 && EventManager.Instance.talismanObtained != null)
        {
            EventManager.Instance.talismanObtained.Invoke(gameObject.GetComponent<Talisman>());
            
        }

         if (EventManager.Instance.outItem != null)
                EventManager.Instance.outItem.Invoke(this);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (EventManager.Instance.inItem != null)
                EventManager.Instance.inItem.Invoke(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (EventManager.Instance.outItem != null)
                EventManager.Instance.outItem.Invoke(this);
        }
    }
}
