using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
using MonsterLove.StateMachine;
using Spine;

public class Enemy : Character
{
    [SerializeField] protected Vector2 idleDurationRange;
   
    [Header("Anim Values")]
    [SerializeField] private EnemyAnim enemyAnim;

    private StateMachine<EnemyState> stateMachine;
    private Tween patrolTween;
    private Tween preBattleTween;

    public delegate void DelEnemyDefeated(Enemy e);
    public static event DelEnemyDefeated OnEnemyDefeated;

    public EnemyState CurrentState => stateMachine.State;
    public SkeletonAnimation SkeletonAnim => skeletonAnim;
    public EnemyAnim EnemyAnim => enemyAnim;

    protected override void Init() {
        base.Init();
        stateMachine = StateMachine<EnemyState>.Initialize(this, EnemyState.None);
        CombineSkinWithWeapon();
    }

    public void ChangeState(EnemyState state, bool instant) {
        if (instant) {
            stateMachine.ChangeState(state, StateTransition.Overwrite);
        }
        else {
            stateMachine.ChangeState(state, StateTransition.Safe);
        }
    }

    public float ChangeToPreBattleState(Vector3 pos) {
        destination = pos;
        stateMachine.ChangeState(EnemyState.PreBattle, StateTransition.Overwrite);

        float waitTime = Vector2.Distance(pos, transform.position) / movementSpeed;
        return waitTime;
    }

    public void ChangeToBattleState() {
        stateMachine.ChangeState(EnemyState.Battle, StateTransition.Overwrite);
    }

    private void OnEnable() {
        UpdatePowerTxt();
        Init();
        ChangeState(EnemyState.Idle, false);

        skeletonAnim.AnimationState.Event += OnAnimEvent;
    }

    private void OnDisable() {
        skeletonAnim.AnimationState.Event -= OnAnimEvent;

        if (defeated) OnEnemyDefeated(this);
    }

    private void CombineSkinWithWeapon() {
        int idNum = 0;
        switch (characterId) {
            case enemy1:
                idNum = 1;
                break;
            case enemy2:
                idNum = 2;
                break;

            default: return;
        }

        var skeleton = skeletonAnim.Skeleton;
        var skeletonData = skeleton.Data;
        var combinedSkin = new Skin("combine-weapon");

        var skinName = string.Format("weapon/weapon0{0}", idNum);
        //Debug.Log(skinName);
        combinedSkin.AddSkin(skeleton.Skin);
        if (skeletonData.FindSkin(skinName) != null) combinedSkin.AddSkin(skeletonData.FindSkin(skinName));
        skeleton.SetSkin(combinedSkin);
        skeleton.SetSlotsToSetupPose();
        
    }

    public void LookAt(Vector3 des) {
        float dir = transform.position.x - des.x;
        if (dir / skeletonAnim.skeleton.ScaleX < 0) skeletonAnim.skeleton.ScaleX = -skeletonAnim.skeleton.ScaleX; 
    }

    public void SetIdleAnim() {
        if (skeletonAnim.state != null) skeletonAnim.state.SetAnimation(0, EnemyAnim.idle.GetAnim(), true);
    }

    public void SetMoveAnim() {
        skeletonAnim.state.SetAnimation(0, EnemyAnim.walk.GetAnim(), true);
    }

    #region State Idle -> Patrol
    protected float idleDuration;
    protected float idleElapsed;

    protected void Idle_Enter() {
        //Debug.Log(this.name + "idle");
        idleElapsed = 0f;
        idleDuration = Random.Range(idleDurationRange.x, idleDurationRange.y);
        SetIdleAnim();
    }

    protected void Idle_Update() {
        idleElapsed += Time.deltaTime;

        if (idleElapsed >= idleDuration) {
            ChangeState(EnemyState.Patrol, false);
        }
    }

    protected Vector3 patrolDes;
    protected Coroutine patrolRoutine;

    protected IEnumerator Patrol_Enter() {
        //Debug.Log(this.name + " patrol");
        if (currentRoom) patrolDes = currentRoom.GetPatrolPosition(this);
        else patrolDes = transform.position;

        LookAt(patrolDes);

        float delta = Vector3.Distance(patrolDes, transform.position) / movementSpeed;
        patrolTween = transform.DOMove(patrolDes, delta, false).SetEase(Ease.Linear);
        SetMoveAnim();

        yield return new WaitForSeconds(delta);
        ChangeState(EnemyState.Idle, false);
    }

    protected void PreBattle_Enter() {
        //Debug.Log(this.name + " preBattle");
        if (patrolTween != null) patrolTween?.Kill();

        LookAt(destination);
        float delta = Vector3.Distance(destination, transform.position) / movementSpeed;
        preBattleTween = transform.DOMove(destination, delta, false).SetEase(Ease.InOutSine);
        SetMoveAnim();
    }

    protected void Battle_Enter() {
        if (preBattleTween != null) preBattleTween?.Kill();
    }
    #endregion

    public void SetCurrentRoom(Room r) { currentRoom = r; }
}

[System.Serializable]
public class EnemyAnim {
    public CharacterAnim idle;
    public CharacterAnim walk;
    public CharacterAnim attack;
}

public enum EnemyState {
    None = 0,
    Idle = 1,
    Patrol = 2,
    PreBattle = 3,
    Battle = 4,
    Death = 5
}
