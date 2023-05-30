using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : Panel
{
    [SerializeField] private WeaponView weaponViewPrefab;
    [SerializeField] private RectTransform content;
    private Dictionary<ItemID, WeaponView> weaponViewDict = new Dictionary<ItemID, WeaponView>();

    public override void Init() {
        base.Init();
        //UpdateWeaponScroll();
    }

    public override void Show() {
        base.Show();
        UpdateWeapon();
        //UpdateWeaponScroll();
    }

    private void UpdateWeapon() {
        if (LevelController.instance) {
            foreach (ItemID id in LevelController.instance.Inventory.ItemList) {
                if (!weaponViewDict.ContainsKey(id)) {
                    WeaponView view = Instantiate(weaponViewPrefab);
                    WeaponData data = DatabaseHolder.instance.WeaponTable.GetData(id);
                    if (data != null) {
                        view.SetIcon(data.Icon);
                    }
                    view.SetId(id);
                    view.RectTransform.SetParent(content, true);
                    view.RectTransform.localScale = Vector3.one;
                    view.RectTransform.position = new Vector3(view.RectTransform.position.x, view.RectTransform.position.y, content.position.z);

                    weaponViewDict.Add(id, view);
                }
            }
        }

        UpdateEquipStatus();
    }

    private void UpdateEquipStatus() {
        ItemID equippedWeapon = (ItemID)LevelController.instance.Hero.WeaponId;
        foreach (KeyValuePair<ItemID, WeaponView> pair in weaponViewDict) {
            pair.Value.SetStatus(false);
        }
        weaponViewDict[equippedWeapon].SetStatus(true);
    }

    protected override void OnEnable() {
        base.OnEnable();
        WeaponView.OnWeaponChange += UpdateEquipStatus;
    }

    private void OnDisable() {
        WeaponView.OnWeaponChange -= UpdateEquipStatus;
    }

    //[SerializeField] private FocusScrollRect weaponScrollRect;
    //private int selectedIndex = -1;
    //private List<int> weapons = new List<int>();
    //private List<ScrollItem> scrollItems = new List<ScrollItem>();

    //private void UpdateWeaponScroll() {
    //    if (LevelController.instance) {
    //        scrollItems.Clear();
    //        foreach (ItemID id in LevelController.instance.Inventory.ItemList) {
    //            scrollItems.Add(new ScrollItem((int)id));
    //        }
    //        weaponScrollRect.UpdateData(scrollItems);
    //    }
    //}
}
