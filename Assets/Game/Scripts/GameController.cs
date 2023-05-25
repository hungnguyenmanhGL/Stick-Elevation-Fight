using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MonsterLove.StateMachine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public static LoadLevelOption pendingLoadLevelOption;

    [Header("Refs")]
    [SerializeField] private AudioManager audioManager;

    [Header("Values")]
    [SerializeField] private float maxLevel;

    private StateMachine<GameState> stateMachine;

    public AudioManager AudioManager => audioManager;

    private void Awake() {
        if (!instance) instance = this;
        else Destroy(this.gameObject);

        DontDestroyOnLoad(this);
    }

    public void LoadGameScene(LoadLevelOption option) {
        if (option.Level <= maxLevel) {
            pendingLoadLevelOption = option;
            StartCoroutine(LoadLevelRoutine(option));
        }
        else {
            LoadHomeScene();
        }
    }
    //load level again for play again button
    public void LoadGameSceneAgain() {
        StartCoroutine(LoadLevelRoutine(pendingLoadLevelOption));
    }
    //load next level
    public void LoadNextLevelGameScene() {
        int nextLevel = pendingLoadLevelOption.Level + 1;
        LoadLevelOption nextLoadOption = LoadLevelOption.Create(nextLevel);
        LoadGameScene(nextLoadOption);
    }

    private IEnumerator LoadLevelRoutine(LoadLevelOption option) {
        yield return SceneManager.LoadSceneAsync(GameScene.ByName.Game);

        LevelController lvlPrefab = Resources.Load<LevelController>(string.Format("Level/Level {0}", option.Level));
        if (lvlPrefab) {
            LevelController lvl = Instantiate(lvlPrefab);
            lvl.Init();
        }
        else { Debug.Log(string.Format("No level {0} found!", option.Level)); }
    }

    public void LoadHomeScene() {
        StartCoroutine(LoadHomeSceneRoutine());
    }

    private IEnumerator LoadHomeSceneRoutine() {
        yield return SceneManager.LoadSceneAsync(GameScene.ByIndex.Home);

        Panel panel = HUD.instance.Push(PanelEnum.home);
    }

    public bool CheckNextLevel() {
        if (pendingLoadLevelOption.Level < maxLevel) return true;
        return false;
    }
}

//NOTE: Level in game resources starts from 1
public struct LoadLevelOption {
    private string path;
    private int level;
    private int skin;
    private int weapon;

    public int Skin => skin;
    public int Weapon => weapon;
    public string Path => path;
    public int Level => level;

    public static LoadLevelOption Create(int level, int skin = 0, int weapon = 0) {
        string path = string.Format("Levels/Level_{0}", level);
        LoadLevelOption option = new LoadLevelOption { level = level, path = path, skin = skin, weapon = weapon };
        return option;
    }
   
}

public enum GameState {
    None = 0,
    Loading,
    Playing,
    Destroyed,
}

public static class GameScene {

    public static class ByIndex {
        public const int Logo = 0;
        public const int Home = 1;
        public const int Game = 2;
    }

    public static class ByName {
        public const string Logo = "Logo";
        public const string Home = "Home";
        public const string Game = "Game";
    }
}