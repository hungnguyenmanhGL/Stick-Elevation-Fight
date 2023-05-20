using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Panel : MonoBehaviour
{
    [SerializeField] protected RectTransform rectTransform;

    private bool initialized = false;
    private HUD hud;

    public RectTransform RectTransform => rectTransform;
    public bool Initialized => initialized;

    virtual public void Init() { 
        initialized = true;
        if (HUD.instance) hud = HUD.instance;
    }
    virtual public void Show() { }
    virtual public void Hide() {
        if (hud) {
            hud.HidePanel(this);
        }
        gameObject.SetActive(false);
    }

    virtual protected void OnEnable() {
        if (!initialized) Init();
    }
}

public enum PanelEnum {
    home = 0,
    spin = 1,
    pause = 2,
    result = 3,
    
}
