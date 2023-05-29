using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : Panel
{
    [SerializeField] private Button inventoryBtn;
    [SerializeField] private Button pauseBtn;

    public delegate bool DelInLevelBtnClicked();
    public static event DelInLevelBtnClicked OnBtnClicked;

    

    public override void Init() {
        base.Init();
        inventoryBtn.onClick.AddListener(OnInventoryBtnClicked);
        pauseBtn.onClick.AddListener(OnPauseBtnClicked);
    }

    public void OnPauseBtnClicked() {
        if (CheckHeroIdleState()) {
            Panel panel = HUD.instance.Push(PanelEnum.pause);
        }
    }
    public void OnInventoryBtnClicked() {

    }

    public bool CheckHeroIdleState() {
        bool idle = false;
        if (OnBtnClicked != null) {
            idle = OnBtnClicked();
        }
        //Debug.Log(idle);
        return idle;
    }
    //[SerializeField] private Button openBtn;
    //[SerializeField] private Button closeBtn;
    //[SerializeField] private GameObject inventoryPanel;

    //[SerializeField] private Button equipBtn;
    //[SerializeField] private FocusScrollRect weaponScrollRect;

    //private int selectedIndex = -1;
    //private List<int> weapons;
    //private List<ScrollItem> scrollItems;


    //private void ShowPanel() {
    //    UpdateWeaponScroll();
    //    bool heroIdle = false;
    //    if (OnInventoryClicked != null) {
    //        heroIdle = OnInventoryClicked();
    //    }
    //    if (heroIdle) inventoryPanel.SetActive(true);
    //}

    //private void UpdateWeaponScroll() {
    //    if (LevelController.instance != null) {
    //        scrollItems = new List<ScrollItem>();
    //        foreach (ItemID id in LevelController.instance.Inventory.ItemList) {
    //            scrollItems.Add(new ScrollItem((int)id));
    //        }
    //        weaponScrollRect.UpdateData(scrollItems);
    //    }
    //}

    //private void ClosePanel() {
    //    inventoryPanel.SetActive(false);
    //}
}
