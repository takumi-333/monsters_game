using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction : Action
{
    public EnemyAction(Monster attacker, Monster defender, int action_id) : base(attacker, defender, action_id){}

    public override void HandleAction() {
        defender.param.hp -= attacker.param.atk;
    }
}
