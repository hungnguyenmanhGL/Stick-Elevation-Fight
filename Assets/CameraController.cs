using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private ProCamera2D proCamera;
    private ProCamera2DNumericBoundaries camBound;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveToHeroSpeed = 8f;
    [SerializeField] private Hero hero;

    [SerializeField] private Bound bound;


    private void Start() {
        cam = Camera.main;
        proCamera = cam.GetComponent<ProCamera2D>();
        camBound = cam.GetComponent<ProCamera2DNumericBoundaries>();
        if (!proCamera || !camBound) {
            Debug.Log("No ProCamera detected!");
            this.enabled = false;
        }

        SetCameraBound();
        MoveToHero();
    }

    private void OnEnable() {
        Room.OnBattleStart += MoveToBattleRoom;
        Room.OnBattleEnd += MoveToHero;
    }

    private void OnDisable() {
        Room.OnBattleStart -= MoveToBattleRoom;
        Room.OnBattleEnd -= MoveToHero;
    }

    private void SetRefs(Hero hero) {
        this.hero = hero;
    }

    //private IEnumerator MoveToPositionRoutine(Vector3 pos) {
    //    while (Vector2.Distance(cam.transform.position, pos) >= 0.0001f) {
    //        cam.transform.position = Vector3.MoveTowards(cam.transform.position, pos, moveSpeed * Time.deltaTime);
    //        yield return 0;
    //    }
    //}

    //private IEnumerator MoveToHeroRoutine() {
    //    while (Vector2.Distance(cam.transform.position, hero.transform.position) >= 0.0001f) {
    //        Vector3 des = hero.transform.position;
    //        des.z = cam.transform.position.z;
    //        cam.transform.position = Vector3.MoveTowards(cam.transform.position, des, moveToHeroSpeed * Time.deltaTime);
    //        yield return 0;
    //    }
    //    positionSet = false;
    //}

    //private void MoveToBattleRoom(Room room) {
    //    Vector3 des = room.GetCellCenterWorldPosition();
    //    des.z = cam.transform.position.z;
    //    positionSet = true;
    //    StartCoroutine(MoveToPositionRoutine(des));
    //}

    //private void MoveToHero() {
    //    StartCoroutine(MoveToHeroRoutine());
    //}

    private void MoveToBattleRoom(Room room) {
        proCamera.RemoveAllCameraTargets();
        CameraTarget target = proCamera.AddCameraTarget(room.transform);
        target.TargetOffset = room.RoomSize / 2;
    }

    private void MoveToHero() {
        proCamera.RemoveAllCameraTargets();
        proCamera.AddCameraTarget(hero.transform);
    }

    //private void FixedUpdate() {
    //    if (!positionSet) {
    //        float halfHeight = cam.orthographicSize;
    //        float halfWidth = cam.aspect * halfHeight;

    //        Vector3 camPos = hero.transform.position;
    //        camPos.z = -10;
    //        cam.transform.position = camPos;
    //    }
    //}

    //public void SetPosition(Vector3 pos) {
    //    positionSet = true;
    //    StartCoroutine(MoveToPositionRoutine(pos));
    //}

    //public void UnsetPosition() { positionSet = false; }

    private void SetCameraBound() {
        camBound.TopBoundary = bound.top;
        camBound.BottomBoundary = bound.bottom;
        camBound.LeftBoundary = bound.left;
        camBound.RightBoundary = bound.right;
    }
}

[System.Serializable]
public class Bound {
    public float top;
    public float bottom;
    public float left;
    public float right;
}
