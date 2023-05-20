using System.Collections;
using UnityEngine;
using Spine.Unity;

[System.Serializable]
public class WeaponData {
    [SerializeField] private ItemID id;
    [SerializeField] private Sprite icon;

    public ItemID Id => id;
    public Sprite Icon => icon;
}
