using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Data/WeaponDatabase")]
public class WeaponDatabase : ScriptableObject
{
    [SerializeField] private List<WeaponData> list = new List<WeaponData>();

    public static WeaponDatabase instance;

    public List<WeaponData> List => list;

    private void Awake() {
        if (!instance) instance = this;
        else Destroy(this);
    }
}
