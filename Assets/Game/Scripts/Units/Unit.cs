using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] protected UnitType type;
    [SerializeField] protected int power;
    [SerializeField] protected TMPro.TextMeshProUGUI powerTxt;

    public UnitType Type => type;
    public int Power => power;

    public void UpdatePowerTxt() {
        if (!powerTxt) return;
        if (power == 0) {
            powerTxt.gameObject.SetActive(false);
            return;
        }

        powerTxt.text = power.ToString();
    }

}


public enum UnitType {
    Hero = 0,
    Enemy = 1,
    Boss = 2,
    Item = 3
}

