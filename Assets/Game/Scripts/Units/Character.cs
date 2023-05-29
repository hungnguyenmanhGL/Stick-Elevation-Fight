using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HAVIGAME.UnityInspector;
using Spine.Unity;
using Spine;

public class Character : Unit
{
    [SerializeField, ConstantField(typeof(Character))] protected string characterId;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected int weaponId = 0;

    [SerializeField] protected SkeletonAnimation skeletonAnim;
    [SerializeField] protected Transform effectTransform;
    [SerializeField] protected Room currentRoom;
    [SerializeField] protected Character target;

    [SerializeField] protected List<AnimEffect> animEffectList;

    protected bool defeated = false;

    public string CharacterId => characterId;
    public int WeaponId => weaponId;
    public Room CurrentRoom => currentRoom;

    public Vector3 destination;

    protected virtual void Init() {

    }

    #region SFX Effect
    protected virtual void OnAnimEvent(TrackEntry entry, Spine.Event e) {
        AnimEffect effect = GetAnimEffect(e.Data.Name);
        if (effect != null && target) {
            GameObject sfx = effect.SFXPrefab;
            if (sfx != null) effect.GetEffectPrefab().transform.position = target.GetRandomEffectPosition();
            AudioClip clip = effect.AudioPrefab;
            if (clip != null) {
                //Debug.Log(string.Format("Event: {0} sound: {1}", e.Data.Name, effect.AudioPrefab.name));
                GameController.instance.AudioManager.Play(clip);
            } 
        }

    }

    protected AnimEffect GetAnimEffect(string eventName) {
        AnimEffect result = null;
        foreach (AnimEffect animEffect in animEffectList) {
            if (animEffect.AnimEvent.Equals(eventName))
                result = animEffect;
        }
        return result;
    }

    public virtual Vector3 GetEffectAnchor() {
        Vector3 pos = effectTransform.position;
        return pos;
    }

    public virtual Vector3 GetRandomEffectPosition() {
        float randX = Random.Range(-.2f, 0.5f);
        float randY = Random.Range(-.2f, 0.5f);
        Vector3 pos = effectTransform.position + new Vector3(randX, randY, 0);
        return pos;
    }
    #endregion

    public void SetRoom(Room room) { this.currentRoom = room; }

    public void AddPower(int amount) { 
        power += amount;
        UpdatePowerTxt();
    }

    protected string GetBattleAnim(Enemy enemy, bool win) {
        string result = "win";
        if (!win) result = "lose";

        return string.Format("{0}/{1}_{2}_weapon{3}", enemy.CharacterId, enemy.CharacterId, result, GetWeaponStringId());
    }

    protected string GetWeaponStringId() {
        if (weaponId < 10) {
            return string.Format("0{0}", weaponId);
        } else return weaponId.ToString();
    }

    protected string GetFormattedStringId(int id) {
        if (id < 10) return string.Format("0{0}", id);
        else return id.ToString();
    }

    public const string none = "";
    public const string hero = "hero";
    public const string enemy0 = "enemy0";
    public const string enemy1 = "enemy1";
    public const string enemy2 = "enemy2";
    public const string enemy3 = "enemy3";
    public const string enemy4 = "enemy4";
    public const string enemy5 = "enemy5";
    public const string enemy6 = "enemy6";
    public const string enemy7 = "enemy7";
    public const string enemy8 = "enemy8";
    public const string enemy9 = "enemy9";
    public const string enemy10 = "enemy10";
    public const string enemy11 = "enemy11";
    public const string enemy12 = "enemy12";
    public const string enemy13 = "enemy13";

    public void SetDefeated() {
        defeated = true;
    }

    public void SetTarget(Character target) {
        this.target = target;
    }
}

//play effect when SpineEvent is called
[System.Serializable]
public class AnimEffect {
    [SerializeField, Spine.Unity.SpineEvent] private string animEvent;
    [SerializeField] private GameObject sfxPrefab;
    [SerializeField] private AudioClip audioPrefab;

    public string AnimEvent => animEvent;
    public GameObject SFXPrefab => sfxPrefab;
    public AudioClip AudioPrefab => audioPrefab;

    public GameObject GetEffectPrefab() {
        GameObject effect = PoolManager.instance.GetObject(sfxPrefab);
        return effect;
    }
}

[System.Serializable]
public class CharacterAnim {
    //save string name to call spine anim
    [SerializeField, Spine.Unity.SpineAnimation] private string[] animList;

    public bool IsNullOrEmpty => animList == null || animList.Length <= 0;

    public string GetAnim() {
        if (animList.Length > 0) return animList[0];
        else {
            Debug.LogWarning("No anim name here");
            return "";
        }
    }

    public string GetRandomAnim() {
        if (animList.Length > 0) {
            int index = Random.Range(0, animList.Length);
            return animList[index];
        }
        return null;
    }
}
