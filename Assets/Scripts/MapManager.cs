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

    public string map_scene_name = "MapScene";

    private float steps;
    private float encount_steps;
    public float speed = 0.02f;

    public float min_encount_steps;
    public float max_encout_steps;
    public int[] player_monster_id_list = {0, 1, 2, 6};

    public int num_player_monster;
    public List<PlayerMonster> player_monsters;
    private MonsterData monster_data;
    private SkillData skill_data;

    private List<List<int>> player_monster_skill_ids;

    private AudioSource audio_source;
    // public AudioClip map_bgm;
    public AudioClip encount_enemy_sound;

    public PlayerController PC;
    public Vector3 player_position;
    // 出入り口
    public AreaDoor area_door;

    private void Awake()
    {
        // instanceがすでにあったら自分を消去する。
        if (instance && this != instance)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        
        // Scene遷移で破棄されなようにする。      
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        encount_steps = Random.Range(min_encount_steps, max_encout_steps);
        steps = 0;
        audio_source = GetComponent<AudioSource>();
        audio_source.Play();
        PC = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        PC.speed = speed;
        area_door = GameObject.Find("AreaDoor").GetComponent<AreaDoor>();

        MCM = new MenuCanvasManager(GameObject.Find("MenuCanvas").GetComponent<Canvas>());

        monster_data = Resources.Load("monster_data") as MonsterData;
        skill_data = Resources.Load("skill_data") as SkillData;
        if(player_monsters != null) {
            num_player_monster = player_monsters.Count;
        } else {
            num_player_monster = player_monster_id_list.Length;
        }
        if (player_monsters == null) {
            player_monsters = new List<PlayerMonster>();
            // 仮の形式
            player_monster_skill_ids = new List<List<int>>();
            List<int> player_monster1_skill_ids = new List<int>(){1,2,3};
            player_monster_skill_ids.Add(player_monster1_skill_ids);
            List<int> player_monster2_skill_ids = new List<int>(){1,2,3,4,5,6,7,8,9,10,11,12,13};
            player_monster_skill_ids.Add(player_monster2_skill_ids);
            List<int> player_monster3_skill_ids = new List<int>(){1,2,5,6,7};
            player_monster_skill_ids.Add(player_monster3_skill_ids);
            List<int> player_monster4_skill_ids = new List<int>(){8,9,13};
            player_monster_skill_ids.Add(player_monster4_skill_ids);
            SetPlayerMonsters();
        }
        PC.can_move = true;
        Debug.Log(player_position);
        if (player_position != new Vector3(0,0,0)) PC.transform.position = player_position;

        SCM = new StatusCanvasManager(GameObject.Find("StatusCanvas").GetComponent<Canvas>(), player_monsters);
    }

    private void SetPlayerMonsters() 
    {
        MonsterData.Param player_monster_param;
        PlayerMonster monster;
        for (int i = 0; i < num_player_monster; i++) {
            player_monster_param = monster_data.sheets[0].list.Find(monster=> monster.id == player_monster_id_list[i]);
            monster = new PlayerMonster(player_monster_param);
            List<Skill> skills = new List<Skill>();
            for (int j = 0; j < player_monster_skill_ids[i].Count; j++)
            {
                Skill add_skill = new Skill(skill_data.sheets[0].list.Find(skill=> skill.id  == player_monster_skill_ids[i][j]));
            }
            monster.SetSkills(skills);
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
            Destroy(this.gameObject);

            // イベントからメソッドを削除
            SceneManager.sceneLoaded -= GameSceneLoaded;
        }
    }

    public void MoveMapScene()
    {
        SceneManager.sceneLoaded += GameSceneLoaded;

        SceneManager.LoadScene(area_door.next_area_scene);

        void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            map_scene_name = area_door.next_area_scene;
            PlayerController pc = 
                GameObject.FindWithTag("Player").GetComponent<PlayerController>();

            AreaDoor next_door = 
                GameObject.Find(area_door.next_area_door).GetComponent<AreaDoor>();
            
            Canvas next_menu_canvas =
                GameObject.Find("MenuCanvas").GetComponent<Canvas>();
            pc.transform.position = next_door.transform.Find("NextPosition").transform.position;
            pc.direction = next_door.next_direction;
            pc.can_move = true;
            pc.speed = speed;
            steps = 0;
            this.PC = pc;
            this.area_door = next_door;
            this.MCM = new MenuCanvasManager(next_menu_canvas);
            // イベントからメソッドを削除
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
        if (area_door.fading) {
            PC.can_move = false;
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
                // statusとsaveボタンを非表示に
                MCM.HandleMenu();
            }

            // status windowを閉じる処理
            SCM.CloseStatusWindow(mousePos);
        }
        if (Input.GetKey(KeyCode.D)) Debug.Log(area_door.next_area_scene);
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
    }
}
