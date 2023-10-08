using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonsterData.Param
{
    public bool isEnemy = false;
    public MonsterData.Param param;
    private Action action;
    private RawImage image;
    private GameObject status_window;
    public List<Skill> skills;
    public int max_hp;
    public int max_mp;

    public Monster(MonsterData.Param param) {
        this.param =  param.ShallowCopy();
        max_hp = param.hp;
        max_mp = param.mp;
        skills = new List<Skill>();
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

    public RawImage GetImage() {
        return image;
    }

    public bool isDead() {
        if (param.hp <= 0) {
            return true;
        }
        return false;
    }

    public void SetStatusWindow(GameObject status_window) 
    {
        this.status_window = status_window;
    }

    public GameObject GetStatusWindow() 
    {
        if (isEnemy) {
            return null;
        } else {
            return status_window;
        }
    }

    public void SetSkills(List<int> skill_ids, SkillData skill_data) 
    {
        Skill skill;
        for (int i = 0; i < skill_ids.Count; i++) {
            skill = new Skill(skill_data.sheets[0].list.Find(skill=> skill.id == skill_ids[i]));
            skills.Add(skill);
        }
    }
}
