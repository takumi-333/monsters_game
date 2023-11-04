using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class FarmData
{
    public FarmMonsterData[] farm_monster_datas;
    public int num_farm_monsters;

    public FarmData () {
        num_farm_monsters = 0;
        farm_monster_datas = new FarmMonsterData[60];
    }

    public void AddMonsterData(PlayerMonster player_monster) {
        FarmMonsterData data = new FarmMonsterData();
        data.uuid = player_monster.uuid;
        data.id = player_monster.id;
        data.name_ja = player_monster.name_ja;
        data.level = player_monster.level;
        data.need_exp = player_monster.need_exp;
        data.total_exp = player_monster.total_exp;
        data.now_exp = player_monster.now_exp;
        data.image_path = player_monster.image_path;
        farm_monster_datas[num_farm_monsters] = data;
        num_farm_monsters++;
    }

    public void ChangeMonsterDataAt(PlayerMonster player_monster, int index) {
        FarmMonsterData data = new FarmMonsterData();
        data.uuid = player_monster.uuid;
        data.id = player_monster.id;
        data.name_ja = player_monster.name_ja;
        data.level = player_monster.level;
        data.need_exp = player_monster.need_exp;
        data.total_exp = player_monster.total_exp;
        data.now_exp = player_monster.now_exp;
        data.image_path = player_monster.image_path;
        farm_monster_datas[index] = data;
    }

    public void RemoveMonsterDataAt(int index) {
        FarmMonsterData[] new_farm_monster_datas = new FarmMonsterData[60];
        for (int i = 0; i < num_farm_monsters; i++) {
            if (i < index) {
                new_farm_monster_datas[i] = farm_monster_datas[i];
            }
            else if (i > index) {
                new_farm_monster_datas[i-1] = farm_monster_datas[i];
            }
        }
        num_farm_monsters--;
        farm_monster_datas = new_farm_monster_datas;
    }

    // public void SetMonsterData(List<PlayerMonster> player_monsters) {
    //     FarmMonsterData data;
    //     for (int i = 0; i < player_monsters.Count; i++) {
    //         data = new FarmMonsterData();
    //         data.uuid = player_monsters[i].uuid;
    //         data.id = player_monsters[i].id;
    //         data.name_ja = player_monsters[i].name_ja;
    //         data.level = player_monsters[i].level;
    //         data.need_exp = player_monsters[i].need_exp;
    //         data.now_exp = player_monsters[i].now_exp;
    //         data.total_exp= player_monsters[i].total_exp;
    //         data.image_path = player_mosnters[i].image_path;
    //         farm_monster_datas[i] = data;
    //     }
    // }

    [System.Serializable]
    public class FarmMonsterData {
        public string uuid;
        public int id;
        public string name_ja;
        public int level;
        public int need_exp;
        public int now_exp;
        public int total_exp;
        public string image_path;

    }

    
}

