using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePanel : Panel
{
    [Header("Sfx")]
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private TextMeshProUGUI sfxToggleTxt;

    [Header("Buttons")]
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button homeBtn;

    public delegate void DelPanelStateChanged();
    public static event DelPanelStateChanged OnPanelStateChange;

    private const string toggleOff = "Off";
    private const string toggleOn = "On";

    public override void Init() {
        base.Init();

        sfxToggle.onValueChanged.AddListener(delegate { OnSfxToggle(); });

        replayBtn.onClick.AddListener(OnReplayBtnClicked);
        homeBtn.onClick.AddListener(OnHomeBtnClicked);
    }

    public void OnSfxToggle() {
        if (!sfxToggle.isOn) sfxToggleTxt.text = toggleOff;
        else sfxToggleTxt.text = toggleOn;

        GameController.instance.AudioManager.ToggleSfx();
    }

    public void OnReplayBtnClicked() {
        GameController.instance.LoadGameSceneAgain();
    }

    public void OnHomeBtnClicked() {
        GameController.instance.LoadHomeScene();
    }

    protected override void OnEnable() {
        base.OnEnable();
        OnPanelStateChange?.Invoke();
    }

    private void OnDisable() {
        OnPanelStateChange?.Invoke();
    }
}
