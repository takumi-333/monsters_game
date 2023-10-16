using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager
{
    
    private MonsterManager MM;
    public List<Action> action_order;
    public List<Action> done_actions;
    public PlayerMonster selecter_monster;
    // public List<EnemyMonster> selected_enemy_monsters;

    public List<PlayerMonster> selecting_monsters;
    // public List<PlayerMonster> selected_monsters;
    // public List<Skill> selected_skills;

    public int selected;
    public bool processing_battle;


    public BattleManager(MonsterManager MM)
    {
        this.MM = MM;
        action_order = new List<Action>();
        done_actions = new List<Action>();
        selected = 0;
        processing_battle = false;
    }

    public string GetStartMessage() 
    {
        string startMessage;
        if (MM.num_enemy_monsters == 1) {
            startMessage = MM.enemy_monsters[0].param.name_ja + "が現れた！";
        } else if (MM.num_enemy_monsters > 1) {
            startMessage = MM.enemy_monsters[0].param.name_ja + "たちが現れた！";
        } else {
            startMessage = "";
            Debug.Log("Error: モンスター出現エラー");
        }
        return startMessage;
    }

    public void StartTurn()
    {
        selecting_monsters =  new List<PlayerMonster>();
        action_order = new List<Action>();
        done_actions = new List<Action>();
        // selected_monsters =  new List<PlayerMonster>();
        // selected_enemy_monsters = new List<EnemyMonster>();
        // selected_skills = new List<Skill>();
        foreach(PlayerMonster monster in MM.player_monsters) {
            if (!monster.isDead) {
                selecting_monsters.Add(monster);
            }
        }
    }

    public void HandleEnemySelect(EnemyMonster enemy_monster)
    {
        MM.UpPlayerMonsterWindow(selecter_monster);
        MM.ClearCursors();
        selecting_monsters.Remove(selecter_monster);
        // selected_enemy_monsters.Add(enemy_monster);
        selecter_monster.GetAction().defender = enemy_monster;
    }


    // 死んだ敵がいたら、生きてる敵にターゲット変更、全員死んでいたらアクション削除
    public void ChangeActionTarget(Monster dead_monster)
    {
        action_order.Remove(dead_monster.action);
        // 生きているプレイヤーモンスターのリスト
        List<PlayerMonster> player_alive_monsters = new List<PlayerMonster>();
        foreach(PlayerMonster player_monster in MM.player_monsters)
        {
            if (!player_monster.isDead) player_alive_monsters.Add(player_monster);
        }
        if (player_alive_monsters.Count <= 0) {
            action_order.Clear();
        }
        // 生きている敵モンスターのリスト
        List<EnemyMonster> enemy_alive_monsters = new List<EnemyMonster>();
        foreach(EnemyMonster enemy_monster in MM.enemy_monsters)
        {
            if (!enemy_monster.isDead) enemy_alive_monsters.Add(enemy_monster);
        }
        if (enemy_alive_monsters.Count <= 0) {
            action_order.Clear();
        }
        // action target変更
        int r;
        foreach(Action action in action_order) {
            if (action.defender == dead_monster) {
                if (action.attacker.isEnemy) {
                    r = Random.Range(0,player_alive_monsters.Count);
                    action.defender = player_alive_monsters[r];
                }
                if (!action.attacker.isEnemy) {
                    r = Random.Range(0,enemy_alive_monsters.Count);
                    action.defender = enemy_alive_monsters[r];
                }
            }
        }
    }


    public void SetEnemyActions()
    {
        List<PlayerMonster> player_alive_monsters = new List<PlayerMonster>();
        foreach(PlayerMonster player_monster in MM.player_monsters)
        {
            if (!player_monster.isDead) player_alive_monsters.Add(player_monster);
        }

        foreach(EnemyMonster enemy_monster in MM.enemy_monsters) {
            if (enemy_monster.isDead) continue;
            int r = Random.Range(0, player_alive_monsters.Count);
            if (enemy_monster.skills.Count == 0) {
                Debug.LogError("Error: some enemy don't have any skills!");
            }
            int s = Random.Range(0, enemy_monster.skills.Count);
            enemy_monster.SetAction(new Action(enemy_monster, player_alive_monsters[r], enemy_monster.skills[s]));
        }
    }
    // 死んでいないモンスターのActionを素早さ順にソート
    public void SetActionOrder() 
    {
        SetEnemyActions();
        action_order = new List<Action> ();
        List<Monster> all_monsters = new List<Monster>();
        for (int i = 0; i < MM.player_monsters.Count; i++) {
            if (!MM.player_monsters[i].isDead) {
                all_monsters.Add(MM.player_monsters[i]);
            }
        }
        for (int i = 0; i < MM.enemy_monsters.Count; i++) {
            if (!MM.enemy_monsters[i].isDead)
            {
                all_monsters.Add(MM.enemy_monsters[i]);
            }
        }
        all_monsters.Sort((m1,m2) => m2.param.sp - m1.param.sp);
        for (int i = 0; i < all_monsters.Count; i++) {
            action_order.Add(all_monsters[i].GetAction());
        }
    }

    public bool CheckBattleLose()
    {
        foreach(PlayerMonster player_monster in MM.player_monsters) {
            if (player_monster.isDead) continue;
            else return false;
        }
        return true;
    }
    public bool CheckBattleWin()
    {
        foreach(EnemyMonster enemy_monster in MM.enemy_monsters) {
            if (enemy_monster.isDead) continue;
            else return false;
        }
        return true;
    }
}