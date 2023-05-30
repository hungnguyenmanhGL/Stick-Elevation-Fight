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
        if (CheckHeroIdleState()) {
            Panel panel = HUD.instance.Push(PanelEnum.inventory);
            panel.Show();
        }
    }

    public bool CheckHeroIdleState() {
        bool idle = false;
        if (OnBtnClicked != null) {
            idle = OnBtnClicked();
        }
        //Debug.Log(idle);
        return idle;
    }
    
}
