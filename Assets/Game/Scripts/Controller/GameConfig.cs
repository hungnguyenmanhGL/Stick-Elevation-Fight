using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    private void Awake() {
        //QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
}
