using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerAction : Action
{
    public PlayerAction(Monster attacker, Monster defender, int action_id) : base(attacker, defender, action_id){}

    public override void HandleAction() {
        Debug.Log($"Enemy HP: " + defender.param.hp);
        defender.param.hp -= attacker.param.atk;
        Debug.Log($"Enemy HP: " + defender.param.hp);
    }
}
