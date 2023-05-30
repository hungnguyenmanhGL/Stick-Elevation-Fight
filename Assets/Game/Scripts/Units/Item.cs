using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Unit
{
    [SerializeField] private ItemID itemID;

    public ItemID ItemID => itemID;

    private void OnEnable() {
        type = UnitType.Item;
        UpdatePowerTxt();
    }

    public virtual string GetItemStringId() {
        if ((int)itemID < 10) {
            return string.Format("weapon0{0}", (int)itemID);
        } else return string.Format("weapon{0}", (int)itemID);
    }
}

public enum ItemID {
    Fist = 0,
    Sword = 1,
    Buster_Sword = 2,
    Halberd = 3,
    Akimbo = 4
}