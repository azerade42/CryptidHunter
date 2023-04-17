using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    bool toggle = false;
    private void Awake()
    {
        Instance = this;
    }

    public void AddHotbar(Item item)
    {
        HotbarItems.Add(item);
    }
    public void AddInventory(Item item)
    {             
        InventoryItems.Add(item);   
    }
    public void ListItems()
    {
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
            var ItemName2 = obj2.transform.Find("ItemName").GetComponent<Text>();
            var ItemIcon2 = obj2.transform.Find("ItemIcon").GetComponent<Image>();
            ItemName.text = item.itemName;
            ItemIcon.sprite = item.icon;
            ItemName2.text = item.itemName;
            ItemIcon2.sprite = item.icon;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toggle = !toggle;
            InventoryScreen.SetActive(toggle);
            Hotbar.SetActive(!toggle);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = toggle;
        }
    }
}
