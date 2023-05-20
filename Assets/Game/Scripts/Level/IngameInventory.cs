using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameInventory : MonoBehaviour
{
    [SerializeField] private List<ItemID> itemList;

    public List<ItemID> ItemList => itemList;

    private void OnEnable() {
        Hero.OnItemCollected += AddItem;
        if (!Has(ItemID.Fist)) AddItem(ItemID.Fist);
    }

    private void OnDisable() {
        Hero.OnItemCollected -= AddItem;
    }

    public bool Has(ItemID id) {
        if (itemList.Contains(id)) return true;
        return false;
    }

    public void AddItem(ItemID id) {
        if (!Has(id)) itemList.Add(id);
        else return;
    }
}
