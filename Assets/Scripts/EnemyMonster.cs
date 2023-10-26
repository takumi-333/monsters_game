using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMonster : Monster
{
    public EnemyMonster(EachMonsterData.Param u_param, MonsterData.Param param) : base (u_param, param){
        isEnemy = true;
    }
}
