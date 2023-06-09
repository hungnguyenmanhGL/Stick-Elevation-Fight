using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class Hero : Character {

    [SerializeField] private SkeletonRootMotion skeletonRootMotion;
    [SerializeField] private HeroState heroState;

    [SerializeField] private Elevator lastElevator;

    [SerializeField] private Transform hip;
    [SerializeField] private HeroAnim heroAnim;

    [SerializeField] protected int skinId = 0;

    //[SerializeField] private List<AnimEffect> animEffectList;

    private bool lvlCompleted = false;
    private Coroutine moveCoroutine;

    public delegate void DelHeroEnterRoom(Room room);
    public static event DelHeroEnterRoom OnHeroEnterRoom;

    public delegate void DelHeroBattle(Vector3 pos);
    public static event DelHeroBattle OnHeroBattle;

    public delegate void DelHeroStatusChanged(HeroState state);
    public static event DelHeroStatusChanged OnHeroStatusChanged;

    public delegate void DelItemCollected(ItemID id);
    public static event DelItemCollected OnItemCollected;

    public delegate void DelHeroDefeated();
    public static event DelHeroDefeated OnHeroDefeated;
   
    public HeroState HeroState => heroState;

    public void MoveToRoom(Room destination) {
        if (heroState != HeroState.Idle) {
            return;
        }

        transform.parent = destination.gameObject.transform;
        this.destination = destination.GetBattlePosition(this);
        //Debug.Log(transform.position);
        moveCoroutine = StartCoroutine(MoveToRoomRoutine(destination));
    }

    private IEnumerator MoveToRoomRoutine(Room room) {
        heroState = HeroState.Moving;
        if (OnHeroStatusChanged != null) {
            OnHeroStatusChanged(HeroState.Moving);
        }

        if (destination.x < transform.position.x) skeletonAnim.skeleton.ScaleX = -1;
        else skeletonAnim.skeleton.ScaleX = 1;

        skeletonAnim.AnimationState.SetAnimation(0, GetRunAnim(), true);
        while (Vector2.Distance(transform.position, destination) >= 0.0001f) {
            heroState = HeroState.Moving;
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            yield return 0;
        }
        skeletonAnim.AnimationState.SetAnimation(0, GetIdleAnim(), true);
        heroState = HeroState.Idle;

        if (room != null) {
            TransferToNewRoom(room);

            heroState = HeroState.Idle;
            if (OnHeroStatusChanged != null) {
                OnHeroStatusChanged(HeroState.Idle);
            }

            if (OnHeroEnterRoom != null) {
                OnHeroEnterRoom(room);
                //=> Room.StartBattle()
            }
        }

        yield return null;
    }

    private void TransferToNewRoom(Room destination) {
        if (currentRoom is Elevator elevator) {
            lastElevator = elevator;
        }

        currentRoom.TransferHero(destination);
        currentRoom = destination;
    }

    public void Battle(Enemy enemy, System.Action OnCompleted) {
        heroState = HeroState.Fighting;
        OnHeroStatusChanged(HeroState.Fighting);
        target = enemy;
        enemy.SetTarget(this);
        StartCoroutine(ResolveBattleResult(enemy, OnCompleted));
    }

    private IEnumerator ResolveBattleResult(Enemy enemy, System.Action OnCompleted) {
        //yield return new WaitForSeconds(0.5f);//enemy atk anim time
        bool win = enemy.Power < power;

        //get related values
        Spine.Animation battleAnim = skeletonAnim.Skeleton.Data.FindAnimation(GetBattleAnim(enemy, win));
        float duration = battleAnim.Duration;

        RootMotionData data = new RootMotionData();
        data.CheckAtAnimStart(hip.position, Vector3.down);

        //send this to enemy to move to battle position
        Vector3 enemyBattlePos = enemy.CurrentRoom.GetBattlePositionForEnemy(this);
        float waitEnemyToGetToPos = enemy.ChangeToPreBattleState(enemyBattlePos);
        yield return new WaitForSeconds(waitEnemyToGetToPos);

        //set battle anim for hero and enemy and wait till anims end
        skeletonAnim.AnimationState.SetAnimation(0, GetBattleAnim(enemy, win), false);
        enemy.ChangeToBattleState();
        enemy.LookAt(transform.position);
        enemy.SkeletonAnim.AnimationState.SetAnimation(0, GetBattleAnim(enemy, win), false);
        yield return new WaitForSeconds(duration);

        //get anim end position
        Vector3 rootMotion = data.CheckAtAnimEnd(hip.position);
        Spine.TrackEntry idleEntry = skeletonAnim.AnimationState.SetAnimation(0, GetIdleAnim(), true);
        idleEntry.MixDuration = 0;
        yield return 0;

        if (win) {
            AddPower(enemy.Power);

            rootMotion.y = 0;
            rootMotion.z = transform.position.z;
            transform.position += rootMotion;//set hero position at animation end pos

            enemy.SetDefeated();
            enemy.gameObject.SetActive(false);
            target = null;
            //move back to elevator after battle if level isnt cleared
            if (!lvlCompleted) {
                heroState = HeroState.Idle;
                MoveToRoom(lastElevator);
            }
            else skeletonAnim.state.SetAnimation(0, heroAnim.dance.GetRandomAnim(), true);

        } else {
            enemy.ChangeState(EnemyState.Idle, false);
            SetDefeated();
            this.gameObject.SetActive(false);

            if (OnHeroDefeated != null) {
                OnHeroDefeated();
            }
        }
        
        OnCompleted?.Invoke();
    }

    private void OnEnable() { 
        type = UnitType.Hero;
        UpdatePowerTxt();

        skeletonAnim.AnimationState.Event += OnAnimEvent;

        LevelController.OnPlayerClickedRoom += MoveToRoom;
        LevelController.OnWin += RespondOnWin;

        InGamePanel.OnBtnClicked += IsHeroIdle;
    }
    private void OnDisable() {
        skeletonAnim.AnimationState.Event -= OnAnimEvent;

        LevelController.OnPlayerClickedRoom -= MoveToRoom;
        LevelController.OnWin -= RespondOnWin;

        InGamePanel.OnBtnClicked -= IsHeroIdle;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Item item = null; 
        collision.gameObject.TryGetComponent<Item>(out item);

        if (item) {
            AddPower(item.Power);
            item.gameObject.SetActive(false);

            if (item is Weapon weapon) {
                CombineSkin(weapon);
                weaponId = (int)weapon.ItemID;
                OnItemCollected(item.ItemID);
            }
        }
    }

    private void RespondOnWin() {
        //Debug.Log("win");
        if (moveCoroutine != null) {
            StopCoroutine(moveCoroutine);
        }
        lvlCompleted = true;
    }

    public void CombineSkin(Item item) {
        string itemStringId = item.GetItemStringId();
        //Debug.Log(itemStringId);
        Skeleton skeleton = skeletonAnim.Skeleton;
        SkeletonData skeletonData = skeleton.Data;
        Skin combinedSkin = new Skin("with-weapon");

        Skin normalSkin = skeletonData.FindSkin(string.Format("skin/skin_{0}", GetFormattedStringId(skinId)));
        string weaponSkinName = string.Format("weapon/{0}_0", itemStringId);
        //Debug.Log(skinName);
        combinedSkin.AddSkin(normalSkin);
        combinedSkin.AddSkin(skeletonData.FindSkin(weaponSkinName));
        skeleton.SetSkin(combinedSkin);
        skeleton.SetSlotsToSetupPose();
    }

    public void CombineSkin(ItemID id) {
        string itemStringId = GetWeaponFormattedStringId((int)id);
        //Debug.Log(itemStringId);
        Skeleton skeleton = skeletonAnim.Skeleton;
        SkeletonData skeletonData = skeleton.Data;
        Skin combinedSkin = new Skin("with-weapon");

        Skin normalSkin = skeletonData.FindSkin(string.Format("skin/skin_{0}", GetFormattedStringId(skinId)));
        string weaponSkinName = string.Format("weapon/{0}_0", itemStringId);
        //Debug.Log(skinName);
        combinedSkin.AddSkin(normalSkin);
        combinedSkin.AddSkin(skeletonData.FindSkin(weaponSkinName));
        skeleton.SetSkin(combinedSkin);
        skeleton.SetSlotsToSetupPose();

        skeletonAnim.AnimationState.SetAnimation(0, GetIdleAnim(), true);
    }

    private string GetRunAnim() {
        return string.Format("run_weapon0{0}", weaponId);

    }

    private string GetIdleAnim() {
        return string.Format("idle/idle_weapon{0}", GetWeaponStringId());
    }

    private bool IsHeroIdle() {
        if (heroState == HeroState.Idle) return true;
        return false;
    }

    public void SetWeapon(ItemID id) {
        weaponId = (int)id;
        CombineSkin(id);
    }
}

[System.Serializable]
public class HeroAnim {
    public CharacterAnim idle;
    public CharacterAnim run;
    public CharacterAnim run_weapon;
    public CharacterAnim attack;
    public CharacterAnim dance;
}

public enum HeroState {
    Idle = 0,
    Moving = 1,
    Fighting = 2
}

public class HeroSkin {
    public const string defaultSkin = "default";
}

public class RootMotionData {
    public Vector3 startRootMotionBonePos;
    public Vector3 startRootMotionBoneScale;

    public Vector3 rootMotion;

    public void CheckAtAnimStart(Vector3 bone, Vector3 scale) {
        startRootMotionBonePos = bone;
        startRootMotionBoneScale = scale;
    }

    public Vector3 CheckAtAnimEnd(Vector3 bonePos) {
        return bonePos - startRootMotionBonePos;
    }
}