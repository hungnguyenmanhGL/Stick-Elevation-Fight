using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponView : InLevelItemView
{
    [SerializeField] private ItemID weaponId;

    public delegate void DelWeaponChanged();
    public static event DelWeaponChanged OnWeaponChange;

    private void Start() {
        equipBtn.onClick.AddListener(Equip);
    }


    public override void Equip() {
        LevelController.instance.Hero.SetWeapon(weaponId);
        OnWeaponChange?.Invoke();
    }

    public void SetId(ItemID id) {
        weaponId = id;
    }
}
