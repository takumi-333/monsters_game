using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SaveMonsterData
{
    public PlayerMonsterData[] monster_datas;
    public float[] position;
    public string map_name;
    public int event1_flg; 

    public SaveMonsterData(List<PlayerMonster> player_monsters) {
        
        if (player_monsters != null) {
            monster_datas = new PlayerMonsterData[player_monsters.Count];
        }
        map_name = "FirstMapScene";
        event1_flg = 1;
    }

    public void SetMonsterData(List<PlayerMonster> player_monsters) {
        PlayerMonsterData data;
        for (int i = 0; i < player_monsters.Count; i++) {
            data = new PlayerMonsterData();
            data.uuid = player_monsters[i].uuid;
            data.id = player_monsters[i].id;
            data.name_ja = player_monsters[i].name_ja;
            data.level = player_monsters[i].level;
            data.need_exp = player_monsters[i].need_exp;
            data.now_exp = player_monsters[i].now_exp;
            data.total_exp= player_monsters[i].total_exp;
            monster_datas[i] = data;
        }
    }

    [System.Serializable]
    public class PlayerMonsterData {
        public string uuid;
        public int id;
        public string name_ja;
        public int level;
        public int need_exp;
        public int now_exp;
        public int total_exp;
    }
}

