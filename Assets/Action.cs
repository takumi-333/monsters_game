using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Action
{
    private Skill skill;
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

        int p_critical = 15;
        int r_critical = Random.Range(0, 100);
        int total_damage = 0;
        float r;
        //クリティカルじゃないとき
        if (r_critical >= p_critical) {
            int min_damage = Random.Range(0,2);
            r = Random.Range(90, 110);
            float damage = ((float)skill.param.loss_hp / 100.0f + 1) * (((float)skill.param.power/ 100) * skill.param.num_attack * attacker.param.atk) / 2.0f;
            Debug.Log("no defense damage = " + damage);
            damage -= (float)defender.param.def / 4;
            total_damage = (int)(damage * r / 100);
            if (total_damage <= 0) {
                total_damage = min_damage;
            }
        } else {
            r = Random.Range(110, 130);
            Debug.Log("クリティカルヒット！");
            float damage = ((float)skill.param.loss_hp / 100.0f + 1) * (((float)skill.param.power / 100) * skill.param.num_attack * attacker.param.atk) / 2.0f;
            total_damage = (int)(damage * r / 100);
        }
        return total_damage;
    }

    public int CalMagicDamage() {
        int total_damage = 0;
        float r;
        int min_damage = Random.Range(0,2);
        // 消費MPの計算
        int loss_mp = (int)((float)attacker.max_mp * (float)skill.param.loss_mp / 100);
        int virtual_def_loss_mp = (int)((float)defender.max_mp * (float)skill.param.loss_mp / 100);

        // 消費MPの反映
        attacker.param.mp -= loss_mp;

        float damage = loss_mp * ((float)skill.param.power/100) - virtual_def_loss_mp *((float)skill.param.power/400);
        r = Random.Range(95,105);
        total_damage = (int)(damage * r / 100);
        if (total_damage <= 0) {
            total_damage = min_damage;
        }
        return total_damage;
    }

    public virtual void HandleAction(TextMeshProUGUI battleMessage) {
        int total_damage;
        switch (skill.skillType) {
            // 攻撃・特技
            case Skill.SkillType.PHYSICAL:
                total_damage = CalAttackDamage();
                battleMessage.text = defender.param.name_ja + "は" + total_damage.ToString() + "ダメージを受けた！";
                defender.param.hp -= total_damage;
                break;
            // 攻撃魔法
            case Skill.SkillType.MAGIC:
                total_damage = CalMagicDamage();
                battleMessage.text = defender.param.name_ja + "は魔法で" + total_damage.ToString() + "ダメージを受けた！";
                defender.param.hp -= total_damage;
                break;
            // 回復魔法
            case Skill.SkillType.HEAL:
                break;
            default:
                Debug.Log("Error: this is unexpected type of skill in HandleAction()");
                break;
        }
        if (defender.param.hp < 0) {
            defender.param.hp = 0;
        }
    }
}
