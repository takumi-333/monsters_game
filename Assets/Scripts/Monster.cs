using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using System.Diagnostics;
using System;
using UnityEngine.UI;

public class Monster
{
    public bool isEnemy = false;
    // public EachMonsterData.Param param;
    public Action action;
    protected RawImage image;
    public List<Skill> skills;

    // status
    public string uuid;
    public int id;
    public int max_hp;
    public int max_mp;
    public int level;
    public int hp;
    public int mp;
    public int atk;
    public int def;
    public int sp;
    public int magic;
    public string image_path;
    public string name_ja;
    public int exp;
    protected State state;
    public bool isDead;

    public bool blinking = false;

    public enum State
    {
        None,
        Poison,
        Sleep,
    }
    
    public Monster(EachMonsterData.Param u_param, MonsterData.Param param) {
        InitStatus(u_param, param);

        // 各モンスター特有のuuidを取得
        Guid UUID = Guid.NewGuid();
        string convertedUUID = UUID.ToString();
        uuid = convertedUUID;
        skills = new List<Skill>();
        image = null;
        state = State.None;
        level = 1;
        isDead = false;
    }

    private void InitStatus(EachMonsterData.Param u_param, MonsterData.Param param)
    {
        // monsterdata から
        id = param.id;
        image_path = param.image_path;
        name_ja = param.name_ja;

        // each monster dataから
        max_hp = u_param.hp;
        max_mp = u_param.mp;
        hp = u_param.hp;
        mp = u_param.mp;
        atk = u_param.atk;
        def = u_param.def;
        magic = u_param.magic;
        sp = u_param.sp;
        exp = u_param.exp;
    }

    public void SetAction(Action action) {
        this.action = action;
    }

    public Action GetAction() {
        return action;
    }

    public void SetImage(RawImage image) {
        this.image = image;
    }

    public RawImage GetImage() 
    {
        return image;
    }

    public void CheckDead() 
    {
        if (hp <= 0) {
            Debug.Log("detect dead: " + name_ja + " died");
            isDead = true;
            return;
        }
        isDead = false;
        return;
    }

    public void AddSkill(Skill new_skill) 
    {
        skills.Add(new_skill);
    }

    public void SetSkills(List<Skill> skills)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            this.skills.Add(skills[i]);
        }
    }

    public bool CheckEnoughMp(int loss_mp)
    {
        if (mp >= loss_mp) return true;
        else return false;
    }

    public bool CheckEnoughHp(int loss_hp)
    {
        if (hp >= loss_hp) return true;
        else return false;
    }

    public bool CheckCanUseSkill(Skill skill)
    {
        if (CheckEnoughHp(skill.param.loss_hp) && CheckEnoughMp(skill.param.loss_mp)) return true;
        else return false;
    }
}
