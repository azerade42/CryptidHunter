using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> HotbarItems = new List<Item>();
    public List<Item> InventoryItems = new List<Item>();
    public Transform HotbarContent;
    public Transform InventoryContent;
    public GameObject InventoryItem;
    public GameObject HotbarItem;
    public GameObject InventoryScreen;
    public GameObject Hotbar;

    public Item equippedItem;


    public string[] descriptionArray;
    bool toggle = false;
    private int index = 0;

    [SerializeField] private ItemDescription itemDescription;

    private void Start()
    {
        Instance = this;
        EventManager.Instance.toggleInventory += ToggleInventory;

        ListItems();
    }

    private void OnDisable()
    {
        EventManager.Instance.toggleInventory -= ToggleInventory;
    }

    public void AddHotbar(Item item)
    {
        HotbarItems.Add(item);
    }
    public void AddInventory(Item item)
    {             
        InventoryItems.Add(item);   
    }

    public void RemoveHotbar(Item item)
    {
        HotbarItems.Remove(item);
    }

    public void ListItems()
    {
        index = 0;
        foreach (Transform item in HotbarContent)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in InventoryContent)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in HotbarItems)
        {
            GameObject obj = Instantiate(HotbarItem, HotbarContent);
            GameObject obj2 = Instantiate(InventoryItem, InventoryContent);
            var ItemName = obj.transform.Find("ItemName").GetComponent<Text>();
            var ItemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            ItemName.text = item.itemName;
            ItemIcon.sprite = item.icon;

            var ItemName2 = obj2.transform.Find("ItemName").GetComponent<Text>();
            var ItemIcon2 = obj2.transform.Find("ItemIcon").GetComponent<Image>();
            var ItemDescription = obj2.transform.Find("ItemDescription").GetComponent<Text>();
            var ItemID = obj2.transform.Find("ItemID").GetComponent<Text>();
            ItemName2.text = item.itemName;
            ItemIcon2.sprite = item.icon;
            ItemDescription.text = item.itemDescription;
            ItemID.text = item.id.ToString();
            index = Convert.ToInt32(ItemID);
            //descriptionArray[index] = ItemDescription.text;
        }
    }

    /*public string GetDescription(int i)
    {
        return descriptionArray[i];
    }*/


    private void ToggleInventory()
    {
        toggle = !toggle;
        InventoryScreen.SetActive(toggle);
        Hotbar.SetActive(!toggle);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = toggle;
        // itemDescription.ResetDescription();
    }



}
