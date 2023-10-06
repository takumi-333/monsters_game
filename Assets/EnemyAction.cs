using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyAction : Action
{
    public EnemyAction(Monster attacker, Monster defender, int action_id) : base(attacker, defender, action_id){}

    public override void HandleAction(TextMeshProUGUI battleMessage) {
        int total_damage = CalDamage();
        battleMessage.text = defender.param.name_ja + "は" + total_damage.ToString() + "ダメージを受けた！";
        defender.param.hp -= total_damage;
        if (defender.param.hp < 0) {
            defender.param.hp = 0;
        }
    }
}
