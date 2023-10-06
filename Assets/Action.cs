using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Action
{
    private ActionData.Param action_param;
    public Monster attacker;
    public Monster defender;
    public int action_id;

    public Action(Monster attacker, Monster defender, ActionData.Param action_param) {
        this.attacker = attacker;
        this.defender = defender;
        this.action_param = action_param.ShallowCopy();
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
            float damage = ((float)action_param.loss_hp / 100.0f + 1) * (((float)action_param.power/ 100) * action_param.num_attack * attacker.param.atk) / 2.0f;
            Debug.Log("no defense damage = " + damage);
            damage -= (float)defender.param.def / 4;
            total_damage = (int)(damage * r / 100);
            if (total_damage <= 0) {
                total_damage = min_damage;
            }
        } else {
            r = Random.Range(110, 130);
            Debug.Log("クリティカルヒット！");
            float damage = ((float)action_param.loss_hp / 100.0f + 1) * (((float)action_param.power / 100) * action_param.num_attack * attacker.param.atk) / 2.0f;
            total_damage = (int)(damage * r / 100);
        }
        return total_damage;
    }

    public int CalMagicDamage() {
        int total_damage = 0;
        float r;
        int min_damage = Random.Range(0,2);
        // 消費MPの計算
        int loss_mp = (int)((float)attacker.param.mp * (float)action_param.loss_mp / 100);
        int virtual_def_loss_mp = (int)((float)defender.param.mp * (float)action_param.loss_mp / 100);

        // 消費MPの反映
        attacker.param.mp -= loss_mp;

        float damage = loss_mp * ((float)action_param.power/100) - virtual_def_loss_mp *((float)action_param.power/400);
        r = Random.Range(95,105);
        total_damage = (int)(damage * r / 100);
        if (total_damage <= 0) {
            total_damage = min_damage;
        }
        return total_damage;
    }

    public virtual void HandleAction(TextMeshProUGUI battleMessage) {
        int total_damage;
        switch (action_param.type) {
            // 攻撃・特技
            case 1:
                total_damage = CalAttackDamage();
                battleMessage.text = defender.param.name_ja + "は" + total_damage.ToString() + "ダメージを受けた！";
                defender.param.hp -= total_damage;
                break;
            // 攻撃魔法
            case 2:
                total_damage = CalMagicDamage();
                battleMessage.text = defender.param.name_ja + "は魔法で" + total_damage.ToString() + "ダメージを受けた！";
                defender.param.hp -= total_damage;
                break;
            // 回復魔法
            case 3:
                break;
            default:
                Debug.Log("Error: this is unexpected type of action in HandleAction()");
                break;
        }
        if (defender.param.hp < 0) {
            defender.param.hp = 0;
        }
    }
}
