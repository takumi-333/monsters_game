using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerAction : Action
{
    public PlayerAction(Monster attacker, Monster defender, int action_id) : base(attacker, defender, action_id){}

    public override void HandleAction() {
        defender.param.hp -= attacker.param.atk;
        if (defender.param.hp < 0) {
            defender.param.hp = 0;
        }
    }
}
