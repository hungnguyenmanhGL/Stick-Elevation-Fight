using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SimpleJSON;

public class LevelController : MonoBehaviour, IPlayerInputListener {
    public static LevelController instance;

    [Header("[InLevel Refs]")]
    [SerializeField] private IngameInventory inventory;
    [SerializeField] private PlayerInputListener playerInputListener;
 
    [Header("[Level Unit Refs]")]
    [SerializeField] private Hero hero;
    [SerializeField] private List<Enemy> enemyList = new List<Enemy>();

    [Header("[Level Layout Refs]")]
    [SerializeField] private Tilemap elevatorTilemap;
    [SerializeField] private Tilemap roomTilemap;
    [SerializeField] private Tilemap entityTilemap;

    [SerializeField] private PathHolder pathHolder;

    [Header("[Data]")]
    [SerializeField, TextArea(10, 20)] private string jsonData;

    private bool resultDecided = false;

    public delegate void DelPlayerClickedRoom(Room room);
    public static event DelPlayerClickedRoom OnPlayerClickedRoom;

    //to Hero.cs to dance 
    public delegate void DelWin();
    public static event DelWin OnWin;

    private Room[] roomArr;
    private Elevator[] elevatorArr;
    private BoxCollider2D[] blockArr;
    private Enemy[] enemyArr;
    private Item[] itemArr;

    public PathHolder PathHolder => pathHolder;
    public IngameInventory Inventory => inventory;

    private void Awake() {
        if (!instance) instance = this;
        else Destroy(this);

        playerInputListener.SetListener(this);
    }

    //called this before instantiate new level
    public void Init() {
        AssignEnemyToRoom();
    }

    public void SaveData() {
        roomTilemap.RefreshAllTiles();
        elevatorTilemap.RefreshAllTiles();
        entityTilemap.RefreshAllTiles();

        roomArr = roomTilemap.GetComponentsInChildren<Room>();
        enemyArr = entityTilemap.GetComponentsInChildren<Enemy>();

        Dictionary<Vector3Int, Room> roomDict = new Dictionary<Vector3Int, Room>();
        Dictionary<Vector3Int, BoxCollider2D> blockDict = new Dictionary<Vector3Int, BoxCollider2D>();

    }

    private void AssignEnemyToRoom() {
        FilterEnemyList();
        roomArr = roomTilemap.GetComponentsInChildren<Room>();
        foreach (Enemy e in enemyList) {
            Vector3 pos = e.transform.position;
            Vector3Int eCellPos = roomTilemap.WorldToCell(pos);
            foreach (Room r in roomArr) {
                //r.SetCellPosition();
                //Debug.Log(r.CellPosition);
                if (e.Type == UnitType.Enemy && eCellPos == r.CellPosition) {
                    r.UnitList.Add(e);
                    e.SetCurrentRoom(r);
                    break;
                }
                if (e.Type == UnitType.Boss && r.Type == RoomType.Big) {
                    if (r.CellPosition.x <= eCellPos.x && eCellPos.x <= r.CellPosition.x + Room.bigRoomExpand.x) {
                        r.UnitList.Add(e);
                        e.SetCurrentRoom(r);
                        break;
                    }
                } 
            }
        }
    }

    public void OnClicked(Vector3 position) {
        Room currentRoom = hero.CurrentRoom;
        Room clickedRoom;

        Vector2 rayOrigin = position;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero, 10f);
        if (hit.collider) {
            if (hit.collider.gameObject.TryGetComponent<Room>(out clickedRoom)) {
                bool allowedMove = currentRoom.CheckConnectionToClickedRoom(clickedRoom);
                if (allowedMove && OnPlayerClickedRoom != null) {
                    OnPlayerClickedRoom(clickedRoom);
                    //=> Hero.MoveToRoom(room)
                }
            }
        }
    }

    private void RemoveDisabledEnemy(Enemy e) {
        enemyList.Remove(e);
        if (enemyList.Count <= 0) PlayerWon();
    }

    private void PlayerLost() {
        if (!resultDecided) {
            Debug.Log("You lost!");
            resultDecided = true;
            Panel panel = HUD.instance.Push(PanelEnum.result);
            if (panel is ResultPanel resultPanel) {
                resultPanel.InitResult(false);
            }
        }
    }

    private void PlayerWon() {
        if (!resultDecided) {
            Debug.Log("You win!");
            resultDecided = true;
            if (OnWin != null) OnWin();
            if (this.enabled) StartCoroutine(GetWinPanel());
        }
    }
    private IEnumerator GetWinPanel() {
        yield return new WaitForSeconds(3f);
        Panel panel = HUD.instance.Push(PanelEnum.result);
        if (panel is ResultPanel resultPanel) {
            resultPanel.InitResult(true);
        }
    }

    private void OnEnable() {
        Hero.OnHeroDefeated += PlayerLost;
        Enemy.OnEnemyDefeated += RemoveDisabledEnemy;

        //AssignEnemyToRoom();
    }

    private void OnDisable() {
        Hero.OnHeroDefeated -= PlayerLost;
        Enemy.OnEnemyDefeated -= RemoveDisabledEnemy;

    }

    private void FilterEnemyList() {
        for (int i = 0; i < enemyList.Count; i++) {
            if (!enemyList[i] || !enemyList[i].gameObject.activeInHierarchy) enemyList.RemoveAt(i);
        }
    }

    private void FindResultPanel() {

    }
}
