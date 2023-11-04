using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMonster : Monster
{
    public int friendly;

    public EnemyMonster(EachMonsterData.Param u_param, MonsterData.Param param) : base (u_param, param){
        isEnemy = true;
    }

    public bool CheckTamed()
    {
        int r = Random.Range(0,100);
        if (r <= friendly) {
            return true;
        } else {
            return false;
        }
    }
    
}
