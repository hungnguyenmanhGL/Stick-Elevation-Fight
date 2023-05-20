using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using TMPro;

public class ResultPanel : Panel
{
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button playAgainBtn;

    [SerializeField] private TextMeshProUGUI resultTMP;
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private ResultAnim resultAnim;

    private static readonly string winTxt = "Level Completed!";
    private static readonly string loseTxt = "You lost!";

    public override void Init() {
        base.Init();
        homeBtn.onClick.AddListener(OnHomeBtnClicked);
        playAgainBtn.onClick.AddListener(OnPlayAgainBtnClicked);
    }

    public void InitResult(bool win) {
        if (win) {
            skeletonGraphic.AnimationState.SetAnimation(0, resultAnim.win.GetRandomAnim(), true);
            resultTMP.text = winTxt;
        }
        else {
            skeletonGraphic.AnimationState.SetAnimation(0, resultAnim.lose.GetAnim(), false);
            resultTMP.text = loseTxt;
        }
    }

    public void OnHomeBtnClicked() {
        GameController.instance.LoadHomeScene();
    }
    public void OnPlayAgainBtnClicked() {
        GameController.instance.LoadGameSceneAgain();
    }
}

[System.Serializable] 
public class ResultAnim {
    public CharacterAnim win;
    public CharacterAnim lose;
}