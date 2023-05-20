using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;
using UnityEngine.UI;

public class HomePanel : Panel
{
    [SerializeField] private SkeletonAnimation skeletonAnim;

    [Header("Level Select Refs")]
    [SerializeField] private TextMeshProUGUI lvlTxtPrefab;
    [SerializeField] private List<TextMeshProUGUI> lvlTxtList;
    [SerializeField] private RectTransform lvlTxtTransform;
    [SerializeField] private Button prevLvlBtn;
    [SerializeField] private Button nextLvlBtn;
    [SerializeField] private Button playBtn;

    private int levelIndex = 0;
    private int maxLevel = 3;

    public override void Init() {
        base.Init();
        for (int i=0; i<maxLevel; i++) {
            TextMeshProUGUI lvlTxt = Instantiate(lvlTxtPrefab);
            lvlTxt.gameObject.SetActive(false);
            lvlTxt.text = string.Format("Level {0}", i+1);
            lvlTxt.rectTransform.SetParent(lvlTxtTransform.parent);
            lvlTxt.rectTransform.localScale = Vector3.one;
            lvlTxt.rectTransform.position = lvlTxtTransform.position;

            lvlTxtList.Add(lvlTxt);
        }

        lvlTxtList[levelIndex].gameObject.SetActive(true);
        if (levelIndex == 0) prevLvlBtn.gameObject.SetActive(false);
        if (levelIndex == maxLevel - 1) nextLvlBtn.gameObject.SetActive(true);

        prevLvlBtn.onClick.AddListener(OnPrevLvlBtnClicked);
        nextLvlBtn.onClick.AddListener(OnNextLvlBtnClicked);
        playBtn.onClick.AddListener(Play);
    }

    public void Play() {
        GameController.instance.LoadGameScene(LoadLevelOption.Create(levelIndex + 1));
    }

    public void OnNextLvlBtnClicked() {
        prevLvlBtn.gameObject.SetActive(true);

        lvlTxtList[levelIndex].gameObject.SetActive(false);
        levelIndex++;
        lvlTxtList[levelIndex].gameObject.SetActive(true);

        if (levelIndex == maxLevel - 1) {
            nextLvlBtn.gameObject.SetActive(false);
        }
    }

    public void OnPrevLvlBtnClicked() {
        nextLvlBtn.gameObject.SetActive(true);

        lvlTxtList[levelIndex].gameObject.SetActive(false);
        levelIndex--;
        lvlTxtList[levelIndex].gameObject.SetActive(true);

        if (levelIndex == 0) {
            prevLvlBtn.gameObject.SetActive(false);
        }
    }
}
