using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InLevelItemView : MonoBehaviour
{
    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] protected Image icon;

    [SerializeField] protected Button equipBtn;
    [SerializeField] protected Image equippedImg;

    public RectTransform RectTransform => rectTransform;

    public void SetIcon(Sprite sprite) {
        icon.sprite = sprite;
    }

    public virtual void Equip() {

    }

    public virtual void SetStatus(bool equipped) {
        if (equipped) {
            equipBtn.gameObject.SetActive(false);
            equippedImg.gameObject.SetActive(true);
        }
        else {
            equipBtn.gameObject.SetActive(true);
            equippedImg.gameObject.SetActive(false);
        }
    }
}
