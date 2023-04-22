using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDescription : MonoBehaviour
{
    [SerializeField] private TMP_Text description;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Item item;
    

    private void Awake()
    {
        ResetDescription();
    }

    public void ResetDescription()
    {
        this.description.text = "";
    }

    public void SetDescription()
    {
        description.text = inventoryManager.GetDescription();
    }
}
