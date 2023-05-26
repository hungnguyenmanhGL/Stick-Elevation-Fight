using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    //[SerializeField] private WeaponID weaponID;

    //public WeaponID WeaponID => weaponID;
}

public enum WeaponID {
    Fist = 0,
    Sword = 1,
    Buster_Sword = 2,
    Halberd = 3,
    Akimbo = 4,
    Rifle = 5
}