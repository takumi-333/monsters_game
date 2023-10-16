using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMonster : Monster
{
    private int exp_next_level;
    private int total_exp;
    private int now_exp;
    private List<int> exp_table;
    private GameObject status_window;

    public bool isFocus = false;

    public PlayerMonster(MonsterData.Param param) : base (param) {
        isEnemy = false;
        total_exp = 0;
        exp_next_level = 50;
        now_exp = 0;
    }

    public void SetExpNextLevel(int exp_next_level) 
    {
        this.exp_next_level = exp_next_level;
    } 

    public int GetExpNextLevel() 
    {
        return exp_next_level;
    } 

    public void SetTotalExp(int total_exp)
    {
        this.total_exp = total_exp;
    }

    public int GetTotalExp() 
    {
        return total_exp;
    } 

    public bool HandleLevelUp(int obtain_exp)
    {
        now_exp += obtain_exp;
        total_exp += obtain_exp;
        if (now_exp > exp_next_level) {
            now_exp -= exp_next_level;
            this.level++;
            return true;
        }
        return false;
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
