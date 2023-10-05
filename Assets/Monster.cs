using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonsterData.Param
{
    public MonsterData.Param param;
    private Action action;

    public Monster(MonsterData.Param param) {
        this.param =  param.ShallowCopy();
    }

    public void SetAction(Action action) {
        this.action = action;
    }

    public Action GetAction() {
        return action;
    }
}
