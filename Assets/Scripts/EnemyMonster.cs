using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMonster : Monster
{
    public EnemyMonster(MonsterData.Param param) : base (param){
        isEnemy = true;
    }
}
