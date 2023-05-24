using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField] protected RoomType roomType;

    [Header("Refs")]
    [SerializeField] protected Tilemap roomTilemap;
    [SerializeField] protected List<Unit> unitList = new List<Unit>();
    [SerializeField] protected Hero hero;

    protected Vector2 roomSize;
    protected Vector3Int cellPosition;

    public static readonly Vector2 bigRoomExpand = new Vector2(1, 2);

    public static readonly Vector2 smallRoomSize = new Vector2(5,3);
    public static readonly Vector2 bigRoomSize = new Vector2(13,9);
    public static readonly Vector2 elevatorSize = new Vector2(3, 3);

    public static readonly Vector2 bigRoomBattleVec = new Vector2(3.5f / 13f, 8.5f / 13f);
    public static readonly Vector2 smallRoomBattleVec = new Vector2(1 / 4f, 3 / 4f);

    //for CameraController.cs to move to room position
    public delegate void DelBroadcastBattleStart(Room room);
    public static event DelBroadcastBattleStart OnBattleStart;
    //for cam to unset position
    public delegate void DelBroadcastBattleEnd();
    public static event DelBroadcastBattleEnd OnBattleEnd;

    protected virtual void OnEnable() {
        switch (roomType) {
            case RoomType.Elevator:
                roomSize = elevatorSize;
                break;
            case RoomType.Small:
                roomSize = smallRoomSize;
                break;
            case RoomType.Big:
                roomSize = bigRoomSize;
                break;
            default:
                break;
        }

        roomTilemap = this.GetComponentInParent<Tilemap>();
        if (roomTilemap) {
            cellPosition = roomTilemap.WorldToCell(transform.position);
        }

        //set room for unit in room
        FilterUnitList();
        foreach (Unit unit in unitList) {
            if (unit is Character character) {
                character.SetRoom(this);
            }
        }

        Hero.OnHeroEnterRoom += StartBattle;
    }

    private void OnDisable() {
        Hero.OnHeroEnterRoom -= StartBattle;
    }

    public void SetHero(Hero hero) {
        this.hero = hero;
    }
    public void TransferHero(Room room) {
        room.SetHero(hero);
        this.hero = null;
    }

    private void StartBattle(Room room) {
        if (room == this && unitList.Count > 0) {
            if (!hero) {
                Debug.LogError("Hero is not transfered yet!");
                return;
            }
            else {
                StartCoroutine(SignalHeroToStartBattle());
            }
        }
    }
    private IEnumerator SignalHeroToStartBattle() {
        if (OnBattleStart != null) OnBattleStart(this);

        for (int i = 0; i < unitList.Count; i++) {
            Enemy enemy = (Enemy)unitList[i];
            while (hero.HeroState != HeroState.Idle) {
                yield return 0;
            }

            bool battleCompleted = false;
            hero.Battle(enemy, ()=> {
                battleCompleted = true;
            });

            while (!battleCompleted) yield return 0;

            if (!enemy.gameObject.activeInHierarchy) {
                unitList.Remove(enemy);
                //Debug.Log("Hero won!");
            }
            else {
                yield return null;
            }
        }
        if (OnBattleEnd != null) OnBattleEnd();
        yield return null;
    }

    virtual public bool CheckConnectionToClickedRoom(Room destination) {
        if (destination is not Elevator) return false;
        else {
            Vector2 destinationCellSize = elevatorSize;
            Vector2 destinationPos = destination.transform.position;

            if (destinationPos.y == transform.position.y) {
                if (destinationPos.x + destinationCellSize.x == transform.position.x) {
                    return true;
                }
                if (transform.position.x + roomSize.x == destinationPos.x) {
                    return true;
                }
            }
        }
        return false;
    }

    virtual public Vector2Int GetConnectionCell() {
        if (this is not Elevator) {
            Vector3 leftDoorPos = transform.position;
            Vector3 rightDoorPos = new Vector3(transform.position.x + roomSize.x, transform.position.y, transform.position.z);

            Vector2Int leftDoorCell = (Vector2Int)LevelController.instance.PathHolder.Tilemap.WorldToCell(leftDoorPos);
            Vector2Int rightDoorCell = (Vector2Int)LevelController.instance.PathHolder.Tilemap.WorldToCell(rightDoorPos);
            //Debug.Log(leftDoorCell + " " + rightDoorCell);
            if (!LevelController.instance.PathHolder.CheckCell(leftDoorCell)) {
                return rightDoorCell;
            }
            return leftDoorCell;
        } else return new Vector2Int(0, 0);
    }

    public Vector3Int GetCurrentCellPosition() {
        return roomTilemap.WorldToCell(transform.position);
    }

    //public Vector3 GetCellCenterWorld() {
    //    return roomTilemap.GetCellCenterWorld(cellPosition);
    //}

    //public Vector3 GetCellCenterLocal() {
    //    return roomTilemap.GetCellCenterLocal(cellPosition);
    //}

    //public Vector3 GetCellCenterPosition() {
    //    if (roomType == RoomType.Small) {
    //        return new Vector3(transform.position.x + roomSize.x / 2, transform.position.y + roomSize.y / 2, 0);
    //    }
    //    else if (roomType == RoomType.PlayerRoom) {
    //        return new Vector3(transform.position.x + playerRoomSize.x / 2, transform.position.y + playerRoomSize.y / 2, 0);
    //    }
    //    return Vector3.zero;
    //}

    public Vector3 GetCellCenterWorldPosition() {
        return new Vector3(transform.position.x + roomSize.x / 2, transform.position.y + roomSize.y / 2, 0);
    }

    public Vector3 GetCellBottomCenterWorldPosition() {
        return new Vector3(transform.position.x + roomSize.x / 2, transform.position.y, 0);
    }

    public Vector3 GetBattlePosition(Hero hero) {
        if (roomType == RoomType.Small) {
            if (hero.transform.position.x < transform.position.x) {
                return new Vector3(transform.position.x + smallRoomSize.x / 4, transform.position.y, hero.transform.position.z);

            } else {
                return new Vector3(transform.position.x + 3*smallRoomSize.x / 4, transform.position.y, hero.transform.position.z);
            }
        } else if (roomType == RoomType.Elevator) {
            return new Vector3(transform.position.x + elevatorSize.x / 2, transform.position.y, hero.transform.position.z);
        }
        else if (roomType == RoomType.Big) {
            if (hero.transform.position.x < transform.position.x) {
                return new Vector3(transform.position.x + roomSize.x / 4, transform.position.y, hero.transform.position.z);

            } else {
                return new Vector3(transform.position.x + 3 * roomSize.x / 4, transform.position.y, hero.transform.position.z);
            }
        }
        return Vector3.zero;
    }

    public Vector3 GetBattlePositionForEnemy(Hero hero) {
        if (roomType == RoomType.Small) {
            Vector3 rightPos = new Vector3(transform.position.x + smallRoomSize.x / 4, transform.position.y, 0); 
            Vector3 leftPos = new Vector3(transform.position.x + 3 * smallRoomSize.x / 4, transform.position.y, 0);

            float disToRight = Mathf.Abs(hero.transform.position.x - rightPos.x);
            float disToLeft = Mathf.Abs(hero.transform.position.x - leftPos.x);
            if (disToRight < disToLeft) return leftPos;
            else return rightPos;
        }
        if (roomType == RoomType.Big) {
            Vector3 rightPos = new Vector3(transform.position.x + roomSize.x * bigRoomBattleVec.x, transform.position.y, 0);
            Vector3 leftPos = new Vector3(transform.position.x + roomSize.x * bigRoomBattleVec.y, transform.position.y, 0);

            float disToRight = Mathf.Abs(hero.transform.position.x - rightPos.x);
            float disToLeft = Mathf.Abs(hero.transform.position.x - leftPos.x);
            if (disToRight < disToLeft) return leftPos;
            else return rightPos;
        }
        return Vector3.zero;
    }

    public Vector3 GetPatrolPosition(Enemy e) {
        if (roomType == RoomType.Small) {
            Vector3 rightPos = new Vector3(transform.position.x + smallRoomSize.x / 4, transform.position.y, e.transform.position.z);
            Vector3 leftPos = new Vector3(transform.position.x + 3 * smallRoomSize.x / 4, transform.position.y, e.transform.position.z);

            float disToRight = Mathf.Abs(e.transform.position.x - rightPos.x);
            float disToLeft = Mathf.Abs(e.transform.position.x - leftPos.x);
            if (disToRight < disToLeft) return leftPos;
            else return rightPos;
        }
        if (roomType == RoomType.Big) {
            Vector3 rightPos = new Vector3(transform.position.x + roomSize.x / 6, transform.position.y, 0);
            Vector3 leftPos = new Vector3(transform.position.x + 5 * roomSize.x / 6, transform.position.y, 0);

            float disToRight = Mathf.Abs(e.transform.position.x - rightPos.x);
            float disToLeft = Mathf.Abs(e.transform.position.x - leftPos.x);
            if (disToRight < disToLeft) return leftPos;
            else return rightPos;
        }
        return Vector3.zero;
    }

    public Vector3 GetCellTopCenterWorldPosition() {
        if (roomType == RoomType.Small) {
            return new Vector3(transform.position.x + smallRoomSize.x / 2, transform.position.y + smallRoomSize.y, 0);
        } else if (roomType == RoomType.Elevator) {
            return new Vector3(transform.position.x + elevatorSize.x / 2, transform.position.y + elevatorSize.y, 0);
        }
        return Vector3.zero;
    }

    private void FilterUnitList() {
        for (int i = 0; i < unitList.Count; i++) {
            if (!unitList[i] || !unitList[i].gameObject.activeInHierarchy) unitList.RemoveAt(i);
        }
    }

    public void SetCellPosition() {
        cellPosition = roomTilemap.WorldToCell(transform.position);
    }

    public RoomType Type => roomType;
    public Vector2 RoomSize => roomSize;
    public Tilemap RoomTilemap => roomTilemap;
    public Vector3Int CellPosition => cellPosition;
    public List<Unit> UnitList => unitList;
}

public enum RoomType {
    Elevator = 0,
    Small = 1,
    Big = 2
}
