using FancyScrollView;
using UnityEngine;
using UnityEngine.UI;

public class ScrollCell : FancyCell<ScrollItem, ScrollContext> {
    //public static readonly int ScrollHash = Animator.StringToHash("scroll");

    //[SerializeField] Animator animator;
    [SerializeField] protected Button button;
    [SerializeField] protected Image image;

    private float currentPosition = 0;

    private void OnEnable() {
        UpdatePosition(currentPosition);
    }

    public override void Initialize() {
        button.onClick.AddListener(OnSelected);
    }

    public override void UpdateContent(ScrollItem itemData) {
        foreach (WeaponData data in DatabaseHolder.instance.WeaponTable.List) {
            if (data.Id == (ItemID)itemData.ItemId) {
                image.sprite = data.Icon;
            }
        }
    }

    public override void UpdatePosition(float position) {
        

    }

    protected void OnSelected() {
       Context.OnCellClicked?.Invoke(Index);
    }
}
