using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public virtual void HandleAction() {
        Debug.Log("Called Parent HandleAction()");
    }
}
