using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Action
{
    public Skill skill;
    public Monster attacker;
    public Monster defender;
    public int skill_id;

    public Action(Monster attacker, Monster defender, Skill skill) {
        this.attacker = attacker;
        this.defender = defender;
        this.skill = skill;
    }

    public int CalAttackDamage() {

        //HP減る処理?
        attacker.hp -= skill.param.loss_hp;
        int p_critical = 15;
        int r_critical = Random.Range(0, 100);
        int total_damage = 0;
        float r;
        //クリティカルじゃないとき
        if (r_critical >= p_critical) {
            int min_damage = Random.Range(0,2);
            r = Random.Range(90, 110);
            float damage = (((float)skill.param.power/ 100) * skill.param.num_attack * attacker.atk) / 2.0f;
            Debug.Log("no defense damage = " + damage);
            damage -= (float)defender.def / 4;
            total_damage = (int)(damage * r / 100);
            if (total_damage <= 0) {
                total_damage = min_damage;
            }
        } else {
            r = Random.Range(110, 130);
            Debug.Log("クリティカルヒット！");
            float damage = (((float)skill.param.power / 100) * skill.param.num_attack * attacker.atk) / 2.0f;
            total_damage = (int)(damage * r / 100);
        }
        return total_damage;
    }

    public int CalMagicDamage() {
        int total_damage = 0;
        float r;
        int min_damage = Random.Range(0,2);

        // 消費MPの反映
        attacker.mp -= skill.param.loss_mp;

        float damage = ((float)skill.param.power/100) *((float)skill.param.power/400) * ((float)(attacker.magic + 100)/(float)(defender.magic+100));
        r = Random.Range(95,105);
        total_damage = (int)(damage * r / 100);
        if (total_damage <= 0) {
            total_damage = min_damage;
        }
        return total_damage;
    }

    public virtual int HandleAction() {
        int total_damage = 0;
        switch (skill.skillType) {
            // 攻撃・特技
            case Skill.SkillType.PHYSICAL:
                total_damage = CalAttackDamage();
                defender.hp -= total_damage;
                break;
            // 攻撃魔法
            case Skill.SkillType.MAGIC:
                total_damage = CalMagicDamage();
                defender.hp -= total_damage;
                break;
            // 回復魔法
            case Skill.SkillType.HEAL:
                total_damage = -1;
                break;
            default:
                Debug.Log("Error: this is unexpected type of skill in HandleAction()");
                break;
        }
        if (defender.hp < 0) {
            defender.hp = 0;
        }
        return total_damage;
    }
}
