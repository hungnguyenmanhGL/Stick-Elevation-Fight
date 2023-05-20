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
    [SerializeField] protected Room currentRoom;

    protected bool defeated = false;


    public string CharacterId => characterId;
    public int WeaponId => weaponId;
    public Room CurrentRoom => currentRoom;

    public Vector3 destination;

    protected virtual void Init() {

    }

    protected virtual void OnAnimEvent(TrackEntry entry, Spine.Event e) {
        if (e.Data != null) { Debug.Log(e.Data.Name); }
    }

    public void SetRoom(Room room) { this.currentRoom = room; }

    public void CombineSkin(Item item) {
        string itemStringId = item.GetItemStringId();
        //Debug.Log(itemStringId);
        var skeleton = skeletonAnim.Skeleton;
        var skeletonData = skeleton.Data;
        var combinedSkin = new Skin("combine-weapon");
        
        var skinName = string.Format("weapon/{0}_0", itemStringId);
        //Debug.Log(skinName);
        combinedSkin.AddSkin(skeleton.Skin);
        combinedSkin.AddSkin(skeletonData.FindSkin(skinName));
        skeleton.SetSkin(combinedSkin);
        skeleton.SetSlotsToSetupPose();
    }

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
