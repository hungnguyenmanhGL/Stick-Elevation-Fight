using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Elevator : Room
{
    [SerializeField] private float movementSpeed = 2f;

    private Coroutine autoMoveCoroutine = null;

    private bool canMove = true;
    private bool isMoving = false;
    //private bool isDragging = false;
    private bool stopSignal = false;

    private Vector3Int lastCellPos;
    private Vector3Int nextCellPos;
    private Vector3 nextPos;

    private PathHolder pathHolder;

    private void Start() {
        roomType = RoomType.Elevator;
        pathHolder = LevelController.instance.PathHolder;
    }

    protected override void OnEnable() {
        base.OnEnable();
        Hero.OnHeroStatusChanged += ChangeStateBasedOnHeroState;
        LevelController.OnPlayerClicked += OnLevelReceivedClick;
        LevelController.OnPlayerClickedAwayRoom += OnPlayerClickedAwayRoom;
    }
    private void OnDisable() {
        Hero.OnHeroStatusChanged -= ChangeStateBasedOnHeroState;
        LevelController.OnPlayerClicked -= OnLevelReceivedClick;
        LevelController.OnPlayerClickedAwayRoom -= OnPlayerClickedAwayRoom;
    }

    private void OnLevelReceivedClick(Vector3 clickedPos) {
       
        if (autoMoveCoroutine != null) {
            stopSignal = true;
            return;
        }

        if (!isMoving && canMove) {
            clickedPos = Camera.main.ScreenToWorldPoint(clickedPos);
            Vector2Int clickedCell = (Vector2Int)pathHolder.Tilemap.WorldToCell(clickedPos);
            //Debug.Log(clickedCell);
            List<Vector2Int> path = pathHolder.GetPath((Vector2Int)GetCurrentCellPosition(), clickedCell);
            if (path != null) {
                autoMoveCoroutine = StartCoroutine(MoveByPath(path));
            }
        }
    }

    private void OnPlayerClickedAwayRoom(Room room) {
        if (autoMoveCoroutine != null) {
            stopSignal = true;
            return;
        }

        if (!isMoving && canMove) {
            StartCoroutine(MoveToRoom(room));
        }
    }

    private void ChangeStateBasedOnHeroState(HeroState heroState) {
        if (heroState != HeroState.Idle) {
            canMove = false;
        } else canMove = true;
    }

    public override bool CheckConnectionToClickedRoom(Room destination) {
        if (destination is Elevator) return false;
        else {
            Vector2 destinationCellSize = smallRoomSize;
            Vector2 destinationPos = destination.transform.position;

            if (destinationPos.y == transform.position.y) {
                if (destinationPos.x + destinationCellSize.x == transform.position.x) {
                    return true;
                }
                if (transform.position.x + roomTilemap.cellSize.x == destinationPos.x) {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckMouseDragDestination(Vector3Int destinationCell) {
        if (Vector3Int.Distance(lastCellPos, destinationCell) <= 1f) {
            Vector2Int raycastDir = (Vector2Int)(destinationCell - GetCurrentCellPosition());
            //Debug.Log(raycastDir);

            Vector2Int desCell = (Vector2Int)GetCurrentCellPosition() + raycastDir;
            if (pathHolder.CheckCell(desCell)) {
                return true;
            }

            return false;

        }
        return false;
    }

    private IEnumerator MoveToNextCellPosition() {
        while (Vector2.Distance(transform.position, nextPos) >= 0.0001f) {
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, nextPos, movementSpeed * Time.deltaTime);
            yield return 0;
        }
        isMoving = false;

        yield return null;
    }

    //NEED : Interception response
    private IEnumerator MoveToRoom(Room room, bool enter = true) {
        List<Vector2Int> path = GetPathToRoom(room);
        bool stopped = false;
        yield return autoMoveCoroutine = StartCoroutine(MoveByPath(path, ()=> { stopped = true; }));
        //if player want to enter room -> check if auto move routine is not stopped before first
        if (!stopped && enter) {
            if (!hero || hero.CurrentRoom != this) yield break;
            hero.MoveToRoom(room);
        }
    }

    private IEnumerator MoveByPath(List<Vector2Int> path, System.Action autoMoveStopped = null) {
        //always start at 2nd count as first in path is always the start cell
        int currentCount = 1;
        while (path != null && Vector2Int.Distance((Vector2Int)GetCurrentCellPosition(), path[path.Count - 1]) >= 0.0001f) {
            nextPos = pathHolder.Tilemap.CellToWorld((Vector3Int)path[currentCount]);
            yield return MoveToNextCellPosition();
            if (stopSignal) {
                stopSignal = false;
                autoMoveCoroutine = null;
                //Debug.Log("Stopped");
                autoMoveStopped?.Invoke();
                yield break;
            }
            currentCount++;
        }
        if (path == null) {
            autoMoveStopped?.Invoke();
        }
        autoMoveCoroutine = null;
    }

    private List<Vector2Int> GetPathToRoom(Room room) {
        Vector2Int leftCell = room.GetLeftConnectedCell();
        List<Vector2Int> path = pathHolder.GetPath((Vector2Int)GetCurrentCellPosition(), leftCell);
        if (path == null) {
            path = pathHolder.GetPath((Vector2Int)GetCurrentCellPosition(), room.GetRightConnectedCell());
            if (path == null) {
                //Debug.Log("no path found");
                return null;
            }
        }
        return path;
    }

    private void OnMouseOver() {
        lastCellPos = GetCurrentCellPosition();
        //isDragging = true;
    }

    private void OnMouseDrag() {
        if (!canMove || isMoving) return;

        //isDragging = true;
        bool allowedMove = true;
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastCellPos = GetCurrentCellPosition();
  
        mousePos.z = 0;
        Vector3Int cellPos = roomTilemap.WorldToCell(mousePos);
        allowedMove = CheckMouseDragDestination(cellPos);
        if (allowedMove) {
            nextCellPos = cellPos;
            nextPos = roomTilemap.CellToWorld(nextCellPos);
            StartCoroutine(MoveToNextCellPosition());
        }
    }

    //private void OnMouseUp() {
    //    isDragging = false;
    //}
}

public enum ElevatorState {
    Idle = 0,
    ManualMove = 1,
    AutoMove = 2
}