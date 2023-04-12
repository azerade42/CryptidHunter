using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();
    public Transform ItemContent;
    public GameObject InventoryItem;

    private bool neverDone;


    private void Awake()
    {
        Instance = this;
        neverDone = true;
    }

    public void Add(Item item)
    {
        if (neverDone)
        { 
            Items.Add(item);
            neverDone = false;
        }
    }
    public void ListItems()
    {
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var ItemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var ItemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            ItemName.text = item.itemName;
            ItemIcon.sprite = item.icon;
        }
    }
}
