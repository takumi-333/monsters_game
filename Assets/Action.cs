using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Action
{
    public Monster attacker;
    public Monster defender;
    public int action_id;

    public Action(Monster attacker, Monster defender, int action_id) {
        this.attacker = attacker;
        this.defender = defender;
        this.action_id = action_id;
    }

    public int CalDamage() {
        int p_critical = 5;
        int r_critical = Random.Range(0, 100);
        int total_damage = 0;
        int r;
        //クリティカルじゃないとき
        if (r_critical >= p_critical) {
            int min_damage = Random.Range(0,2);
            r = Random.Range(90, 110);
            total_damage = attacker.param.atk / 2 - defender.param.def / 4;
            total_damage = total_damage * r / 100;
            if (total_damage <= 0) {
                total_damage = min_damage;
            }
        } else {
            r = Random.Range(110, 130);
            total_damage = (attacker.param.atk / 2) * r / 100;
        }

        return total_damage;
    }

    public virtual void HandleAction(TextMeshProUGUI battleMessage) {
        Debug.Log("Called Parent HandleAction()");
    }
}
