using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class PlayerAction : Action
{
    public PlayerAction(Monster attacker, Monster defender, Skill skill) : base(attacker, defender, skill){}

    // public override void HandleAction(TextMeshProUGUI battleMessage) {
    //     int total_damage = CalDamage();
    //     battleMessage.text = defender.param.name_ja + "は" + total_damage.ToString() + "ダメージを受けた！";
    //     defender.param.hp -= total_damage;
    //     if (defender.param.hp < 0) {
    //         defender.param.hp = 0;
    //     }
    // }
}
