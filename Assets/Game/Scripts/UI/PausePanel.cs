using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : Panel
{
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button homeBtn;

    public override void Init() {
        base.Init();
        replayBtn.onClick.AddListener(OnReplayBtnClicked);
        homeBtn.onClick.AddListener(OnHomeBtnClicked);
    }

    public void OnReplayBtnClicked() {
        GameController.instance.LoadGameSceneAgain();
    }

    public void OnHomeBtnClicked() {
        GameController.instance.LoadHomeScene();
    }
}
