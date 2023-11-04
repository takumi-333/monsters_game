using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstEvent : MonoBehaviour
{
    private MapManager MM;

    public string message1 = "この世界は";
    public string message2 = "私のものだ！！！！";
    
    private List<PlayerMonster> first_player_monsters;
    private List<EnemyMonster> boss_monsters;
    private MonsterData monster_data;
    private EachMonsterData each_monster_data;
    private bool talking;
    private MessageWindowManager MWM;


    void Start()
    {
        monster_data = Resources.Load("monster_data") as MonsterData;
        each_monster_data = Resources.Load("each_monster_data") as EachMonsterData;

        first_player_monsters = new List<PlayerMonster>();
        
        boss_monsters = SetBossMonster();
        SetFirstPlayerMonster();
        

        Debug.Log("First Event start");
        MM = gameObject.GetComponent<MapManager>();
        MM.PC.can_move = false;
        MM.PC.direction = PlayerController.Direction.UP;
        MM.PC.StopWalking();
        MWM = new MessageWindowManager(GameObject.FindWithTag("MessageWindowCanvas").GetComponent<Canvas>());
        MWM.DisplayCanvas();
        talking = true;
        StartCoroutine("DisplayMessage");
    }
    public void SetFirstPlayerMonster()
    {
        int[] id = {11,12,13,14};
        int[] level = {10, 10, 10, 10};
        string[] name = {"オクパ", "ドラゴ", "クック", "バター"};
        PlayerMonster pmon;
        for (int i = 0; i < 4; i++) {
            MonsterData.Param param = monster_data.sheets[0].list.Find(param=>param.id == id[i]);
            EachMonsterData.Param u_param = each_monster_data.sheets.Find(sheet=>sheet.name==id[i].ToString()).list.Find(param=>param.lv==level[i]);
            pmon = new PlayerMonster(u_param, param);
            pmon.name_ja = name[i];
            pmon.hp = 1;
            first_player_monsters.Add(pmon);
        }
    }


    public List<EnemyMonster> SetBossMonster()
    {
        List<EnemyMonster> boss_monsters = new List<EnemyMonster>();
        int[] id = {10};
        int[] level = {10};
        EnemyMonster emon;
        for (int i = 0; i < 1; i++) {
            MonsterData.Param param = monster_data.sheets[0].list.Find(param=>param.id == id[i]);
            Debug.Log("ok");
            EachMonsterData.Param u_param = each_monster_data.sheets.Find(sheet=>sheet.name==id[i].ToString()).list.Find(param=>param.lv==level[i]);
            emon = new EnemyMonster(u_param, param);
            boss_monsters.Add(emon);
        }
        return boss_monsters;
    }

    private IEnumerator DisplayMessage()
    {
        StartCoroutine(MWM.DisplayMessage1(message1));
        yield return new WaitUntil(() => !MWM.displaying);
        StartCoroutine(MWM.DisplayMessage2(message2));
        yield return new WaitUntil(() => !MWM.displaying);
        yield return new WaitForSeconds(1.0f);
        talking = false;
        MWM.ClearAll();
        MM.event2_flg = 1;
        // MM.PC.can_move = true;
        // MM.PC.StartWalking();
        CallBattleScene();
        
    }

    private void CallBattleScene()
    {
            // イベントにメソッドを登録
            SceneManager.sceneLoaded += GameSceneLoaded;

            SceneManager.LoadScene("BattleScene");

        void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            // シーン切り替え後のスクリプトを取得
            var gameManager = 
                GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

            // データを渡す処理
            gameManager.player_position = MM.player_position;
            gameManager.map_scene_name = MM.map_scene_name;
            gameManager.player_monsters = first_player_monsters;
            gameManager.enemy_monsters = boss_monsters;
            gameManager.lose_event = true;
            gameManager.event2_flg = 1;
            gameManager.boss_battle = true;

            // イベントからメソッドを削除
            SceneManager.sceneLoaded -= GameSceneLoaded;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 mousePos;
        // skip不可
        // if (talking) {
        //     if (Input.GetMouseButtonDown(0)) {
        //         MWM.skip = true;
        //     }
        // }

    }
}
