using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseHolder : MonoBehaviour
{
    public static DatabaseHolder instance;

    [SerializeField] private WeaponDatabase weaponTable;

    public WeaponDatabase WeaponTable => weaponTable;

    void Start()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        //Debug.Log(abilityEffectDatabase);
    }
}
