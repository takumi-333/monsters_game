using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMonster : Monster
{
    public int need_exp;
    public int total_exp;
    public int now_exp;
    // private List<int> exp_table;
    private GameObject status_window;

    // battle sceneで使う変数
    public bool isFocus = false;
    public bool level_up = false;

    public PlayerMonster(EachMonsterData.Param u_param, MonsterData.Param param) : base (u_param, param) {
        isEnemy = false;
        need_exp = u_param.need_exp;
        now_exp = 0;
        total_exp = 0;
    }

    public void SetNewStatus(EachMonsterData.Param param)
    {
        max_hp = param.hp;
        max_mp = param.mp;
        hp = param.hp;
        mp = param.mp;
        sp = param.sp;
        atk = param.atk;
        def = param.def;
        magic = param.magic;
    }
    

    public void SetStatusWindow(GameObject status_window) 
    {
        this.status_window = status_window;
    }

    public GameObject GetStatusWindow() 
    {
        if (isEnemy) {
            return null;
        } else {
            return status_window;
        }
    }

}
