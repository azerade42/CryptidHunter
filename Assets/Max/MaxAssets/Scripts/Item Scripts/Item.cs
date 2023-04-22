using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName= "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    public string id;
    public string itemName;
    public Sprite icon;
    public string itemDescription;
    public ItemType itemType;


    public enum ItemType
    {
        Talisman,
        Rifle
    }
}
