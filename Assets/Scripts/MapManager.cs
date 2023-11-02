using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    public static MapManager Instance => instance;

    private MenuCanvasManager MCM;
    private StatusCanvasManager SCM;
    private SaveDataManager SDM;
    private MessageWindowManager MWM;

    public string map_scene_name = "MapScene";

    private float steps;
    private float encount_steps;
    public float speed = 0.02f;

    public float min_encount_steps;
    public float max_encout_steps;
    public SaveMonsterData load_data;

    public int num_player_monster;
    public List<PlayerMonster> player_monsters;

    private MonsterData monster_data;
    private SkillData skill_data;
    private EachMonsterData each_monster_data;

    private List<List<int>> player_monster_skill_ids;

    private AudioSource audio_source;
    // public AudioClip map_bgm;
    public AudioClip encount_enemy_sound;

    public PlayerController PC;
    public Vector3 player_position;
    // 出入り口
    public AreaDoor area_door;

    void Start()
    {
        encount_steps = Random.Range(min_encount_steps, max_encout_steps);
        steps = 0;
        audio_source = GetComponent<AudioSource>();
        audio_source.Play();
        PC = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        PC.speed = speed;
        // area_door = GameObject.Find("AreaDoor").GetComponent<AreaDoor>();

        MCM = new MenuCanvasManager(GameObject.Find("MenuCanvas").GetComponent<Canvas>());
        SDM = new SaveDataManager();
        MWM = new MessageWindowManager(GameObject.FindWithTag("MessageWindowCanvas").GetComponent<Canvas>());

        monster_data = Resources.Load("monster_data") as MonsterData;
        skill_data = Resources.Load("skill_data") as SkillData;
        each_monster_data = Resources.Load("each_monster_data") as EachMonsterData;

        // モンスターがいればnum_player_monsterはplayer_monstersの数に設定
        if(player_monsters != null) {
            num_player_monster = player_monsters.Count;
        }
        /* 
        * モンスターがいない場合
        * load_dataがある->monster_datasがある->num_player_monster, player_monstersのセット
        * load_dataがある->monster_datasがない->初期データを作成する
        * load_dataがnull->初期データを作成する
        */
        if (player_monsters == null) {
            // player_monstersのlist生成
            player_monsters = new List<PlayerMonster>();
            if (load_data != null) {
                if (load_data.monster_datas.Length == 0) {
                    num_player_monster = 1;
                    MonsterData.Param param = monster_data.sheets[0].list.Find(monster=>monster.id == 1);
                    EachMonsterData.Param u_param = each_monster_data.sheets.Find(sheet=>sheet.name=="1").list.Find(param=>param.lv==1);
                    PlayerMonster player_monster = new  PlayerMonster(u_param, param);
                    player_monsters.Add(player_monster);
                }else {
                    num_player_monster = load_data.monster_datas.Length;
                    SetPlayerMonsters();
                }
            }
            else {
                num_player_monster = 1;
                MonsterData.Param param = monster_data.sheets[0].list.Find(monster=>monster.id == 1);
                EachMonsterData.Param u_param = each_monster_data.sheets.Find(sheet=>sheet.name=="1").list.Find(param=>param.lv==1);
                PlayerMonster player_monster = new  PlayerMonster(u_param, param);
                player_monsters.Add(player_monster);
            }
            
            // 仮の形式
            // List<int> player_monster1_skill_ids = new List<int>(){1,2,3};
            // player_monster_skill_ids.Add(player_monster1_skill_ids);
            // List<int> player_monster2_skill_ids = new List<int>(){1,2,3,4,5,6,7,8,9,10,11,12,13};
            // player_monster_skill_ids.Add(player_monster2_skill_ids);
            // List<int> player_monster3_skill_ids = new List<int>(){1,2,5,6,7};
            // player_monster_skill_ids.Add(player_monster3_skill_ids);
            // List<int> player_monster4_skill_ids = new List<int>(){8,9,13};
            // player_monster_skill_ids.Add(player_monster4_skill_ids);
        }

        PC.can_move = true;
        // player_positionが存在すれば、そこに配置
        if (player_position != new Vector3(0,0,0)) PC.transform.position = player_position;

        SCM = new StatusCanvasManager(GameObject.Find("StatusCanvas").GetComponent<Canvas>());
    }

    // Title sceneからload_dataを受け取った時に生成
    private void SetPlayerMonsters() 
    {
        EachMonsterData.Param player_monster_u_param;
        MonsterData.Param player_monster_param;
        SaveMonsterData.PlayerMonsterData data;
        PlayerMonster monster;
        // List<Skill> skills = new List<Skill>();
        for (int i = 0; i < num_player_monster; i++) {
            data = load_data.monster_datas[i];
            player_monster_param = monster_data.sheets[0].list.Find(monster=> monster.id == data.id);
            player_monster_u_param = each_monster_data.sheets.Find(sheet => sheet.name == data.id.ToString()).list.Find(param => param.lv == data.level);
            monster = new PlayerMonster(player_monster_u_param, player_monster_param);

            // load_dataからセット
            monster.level = data.level;
            monster.name_ja = data.name_ja;
            monster.uuid = data.uuid;
            monster.need_exp = data.need_exp;
            monster.total_exp = data.total_exp;
            monster.now_exp = data.now_exp;
            
            // for (int j = 0; j < player_monster_skill_ids[i].Count; j++)
            // {
            //     Skill add_skill = new Skill(skill_data.sheets[0].list.Find(skill=> skill.id  == player_monster_skill_ids[i][j]));
            // }
            // monster.SetSkills(skills);
            player_monsters.Add(monster);
        }
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
            gameManager.player_position = player_position;
            gameManager.map_scene_name = map_scene_name;
            gameManager.player_monsters = player_monsters;

            // イベントからメソッドを削除
            SceneManager.sceneLoaded -= GameSceneLoaded;
        }
    }

    private void InitThis()
    {
        // map_scene_name = area_door.next_area_scene;
        PlayerController pc = 
            GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        MapManager MM =
            GameObject.FindWithTag("MapManager").GetComponent<MapManager>();
        AreaDoor next_door = 
            GameObject.Find(area_door.next_area_door).GetComponent<AreaDoor>();
        
        pc.transform.position = next_door.transform.Find("NextPosition").transform.position;
        pc.direction = next_door.next_direction;
        MM.steps = 0;
        MM.player_monsters = player_monsters;
        MM.PC = pc;
        MM.area_door = next_door;
        // MM.MCM = new MenuCanvasManager(GameObject.Find("MenuCanvas").GetComponent<Canvas>());
        // MM.SCM = new StatusCanvasManager(GameObject.Find("StatusCanvas").GetComponent<Canvas>());
    }

    public void MoveMapScene()
    {
        SceneManager.sceneLoaded += GameSceneLoaded;

        SceneManager.LoadScene(area_door.next_area_scene);

        void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            InitThis();
            SceneManager.sceneLoaded -= GameSceneLoaded;
        }
    }

    private IEnumerator TransBattleScene() {
        audio_source.Stop();
        audio_source.PlayOneShot(encount_enemy_sound);
        yield return new WaitForSeconds (1.0f);
        CallBattleScene();
    }


    // Update is called once per frame
    void Update()
    {
        if (area_door != null) {
            if (area_door.fading) {
                PC.can_move = false;
            }
        }
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            // Menuのクリック処理
            if (MCM.ClickMenuButton(mousePos)) {
                MCM.HandleMenu();
            }
            else if (MCM.ClickSaveButton(mousePos)) {
                MCM.HandleSave(this);
                StartCoroutine(MCM.DisplaySavedMessage());
            } else if (MCM.ClickStatusButton(mousePos)) {
                SCM.OpenStatusWindow();
                SCM.SetPlayerMonstersStatus(player_monsters);
                // statusとsaveボタンを非表示に
                MCM.HandleMenu();
            }

            // status windowを閉じる処理
            SCM.CloseStatusWindow(mousePos);
        }
        if (PC.can_move) {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || 
            Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)) {
                steps += speed;
                player_position = PC.transform.position;
            }
            if (steps >= encount_steps) {
                Debug.Log("Encount!!");
                PC.can_move = false;
                encount_steps = Random.Range(min_encount_steps, max_encout_steps);
                steps = 0;
                StartCoroutine("TransBattleScene");
            }
        }

        if (Input.GetKey(KeyCode.A)) {
            SaveMonsterData load_data = MCM.GetSaveData();
            Debug.Log(load_data.monster_datas.Length);
        }
    }
}
