using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster
{
    public bool isEnemy = false;
    public MonsterData.Param param;
    public Action action;
    protected RawImage image;
    public List<Skill> skills;
    public int max_hp;
    public int max_mp;
    public int level;
    protected State state;
    public bool isDead;

    public bool blinking = false;

    public enum State
    {
        None,
        Poison,
        Sleep,
    }
    
    public Monster(MonsterData.Param param) {
        this.param =  param.ShallowCopy();
        max_hp = param.hp;
        max_mp = param.mp;
        skills = new List<Skill>();
        image = null;
        state = State.None;
        level = 1;
        isDead = false;
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
        if (param.hp <= 0) {
            Debug.Log("detect dead: " + param.name_ja + " died");
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
        if (param.mp >= loss_mp) return true;
        else return false;
    }

    public bool CheckEnoughHp(int loss_hp)
    {
        if (param.hp >= loss_hp) return true;
        else return false;
    }

    public bool CheckCanUseSkill(Skill skill)
    {
        if (CheckEnoughHp(skill.param.loss_hp) && CheckEnoughMp(skill.param.loss_mp)) return true;
        else return false;
    }
}
