using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    private double _time;
    private double turn_wait_time = 0;
    GameObject Screen;
    private MonsterData monster_data;

    private List<RawImage> enemy_images = new List<RawImage>();

    // player's monster info
    private int[] pMonster_id_list = {0, 1, 2, 3};
    private int num_pMonster = 4;
    // private List<MonsterData.Param> pMonsters = new List<MonsterData.Param>();
    private List<Monster> pMonsters = new List<Monster>();
    private List<GameObject> status_windows = new List<GameObject>();

    // enemy monster info
    private List<Monster> enemy_monsters = new List<Monster>();
    private MonsterData.Param monster_param;
    private int num_monster;

    private GameObject command_window;
    private List<GameObject> command_blocks = new List<GameObject>();

    //仮のボタン、これを押すと戦闘開始
    private GameObject start_button;

    private SceneType SceneMode = SceneType.SELECT; 

    // ATTACK時に使用する変数
    private int select_monster;
    private List<Monster> select_monsters;
    private float cursor_blink_rate = 0.5f;
    private int selecter = 0;
    

    // PROCESS時に使用する変数
    private int scene_step = 0;
    private List<Action> action_order;
    private List<Action> done_actions;
    private float damage_blink_rate = 0.5f;
    private double damage_blink_time = 0;


    public enum CommandType
    {
        ATTACK = 0,
        ITEM = 1,
        SPECIAL = 2,
        ESCAPE = 3
    }

    public enum SceneType
    {
        SELECT,
        ATTACK,
        ITEM,
        SPECIAL,
        ESCAPE,
        PROCESS,
        END,
    }

    public string[] command_block_str = {"attack", "item", "special", "escape"};

    // Start is called before the first frame update
    void Start()
    {
        command_window = GameObject.Find("CommandWindow");
        start_button = GameObject.Find("GenerateButton");
        monster_data = Resources.Load("monster_data") as MonsterData;
        Screen = GameObject.Find("BackgroundImage");
    }

    // status windowもmonsterのパラメータもセットする関数
    public void SetPlayerMonsters() 
    {
        
        // set player's monster parameter
        MonsterData.Param pMonster_param;
        Monster monster;
        for (int i = 0; i < num_pMonster; i++) {
            pMonster_param = monster_data.sheets[0].list.Find(monster=> monster.id == pMonster_id_list[i]);
            monster = new Monster(pMonster_param);
            pMonsters.Add(monster);
        }

        // set status window
        GameObject status_window_prefab = Resources.Load<GameObject>("StatusWindow");
        Vector3 pos;
        GameObject status_window;
        for (int i = 0; i < num_pMonster; i++) {
            status_window = Instantiate(status_window_prefab);
            status_window.transform.SetParent(Screen.transform);
            pos = new Vector3(-250+166.7f*i, 150, 0);
            status_window.transform.localPosition = pos;
            status_windows.Add(status_window);
            pMonsters[i].SetImage(status_window.transform.Find("MonsterImage").GetComponent<RawImage>());
            pMonsters[i].SetStatusWindow(status_window);
        }

        for (int i = 0; i < num_pMonster; i++) {
            status_windows[i].transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = pMonsters[i].param.name_en;
            status_windows[i].transform.Find("MonsterImage").GetComponent<RawImage>().texture = Resources.Load<Texture2D>(pMonsters[i].param.image_path);
        }

        UpdateStatusWindow();
    }

    public void UpdateStatusWindow()
    {
        for (int i = 0; i < num_pMonster; i++) {
            pMonsters[i].GetStatusWindow().transform.Find("HpText").GetComponent<TextMeshProUGUI>().text = $"HP: {pMonsters[i].param.hp}";
            pMonsters[i].GetStatusWindow().transform.Find("MpText").GetComponent<TextMeshProUGUI>().text = $"MP: {pMonsters[i].param.mp}";
        }
    }

    public void UpdateStatusWindowDead(Monster pMonster)
    {
        pMonster.GetStatusWindow().GetComponent<Outline>().effectColor = Color.red;
        pMonster.GetStatusWindow().transform.Find("NameText").GetComponent<TextMeshProUGUI>().color = Color.red;
        pMonster.GetStatusWindow().transform.Find("HpText").GetComponent<TextMeshProUGUI>().color = Color.red;
        pMonster.GetStatusWindow().transform.Find("MpText").GetComponent<TextMeshProUGUI>().color = Color.red;
    }

    // 出現するモンスターを出現確率から決定
    public void ChooseMonsterByWeight() 
    {
        int total_weight = 0;
        for (int i = 0; i < monster_data.sheets[0].list.Count; i++) {
            total_weight += monster_data.sheets[0].list[i].weight;
        }
        int r = Random.Range(1, total_weight);
        for (int i = 0; i < monster_data.sheets[0].list.Count; i++) {
            if (r < monster_data.sheets[0].list[i].weight) {
                monster_param = monster_data.sheets[0].list[i];
                break;
            }
            r -= monster_data.sheets[0].list[i].weight;
        }
    }

    public int ClickCommandBlock(Vector3 mousePos) 
    {
        GameObject command_prefab = Resources.Load<GameObject>("Command");
        Vector2 command_block_size = command_prefab.GetComponent<RectTransform>().sizeDelta;
        int index = -1;

        foreach(GameObject command_block in command_blocks) {
            index += 1;
            Vector3 relativeMousePos = command_block.transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= -(command_block_size.x / 2) && relativeMousePos.x <= command_block_size.x / 2) &&
            (relativeMousePos.y >= -(command_block_size.y / 2) && relativeMousePos.y <= command_block_size.y / 2)
            ) {
                return index;
            }
        }

        return -1;
    }

    public void HandleCommandSelect(int command_id)
    {
        // 押されたコマンドブロック以外のブロックを非表示
        for (int i = 0; i < 4; i++) {
            if (i != command_id) {
                command_blocks[i].SetActive(false);
            }
        }

        switch (command_id) {
            case 0:
                SceneMode = SceneType.ATTACK;
                if (selecter == 0) {
                    select_monsters = new List<Monster>();
                }
                select_monster = 0;
                break;
            case 1:
                SceneMode = SceneType.ITEM;
                break;
            case 2:
                SceneMode = SceneType.SPECIAL;
                break;
            case 3:
                SceneMode = SceneType.ESCAPE;
                break;
            default:
                break;
        }
    }

    // Enemyに対するマウスフォーカスの処理
    public void FocusEnemy(Vector3 mousePos) 
    {
        int index = -1;
        foreach(RawImage enemy_image in enemy_images) {
            index += 1;
            Vector3 relativeMousePos = enemy_image.transform.InverseTransformPoint(mousePos);
            Vector2 size = enemy_image.rectTransform.sizeDelta;
            if ((relativeMousePos.x >= -(size.x / 2) && relativeMousePos.x <= size.x / 2) &&
            (relativeMousePos.y >= -(size.y / 2) && relativeMousePos.y <= size.y / 2)
            ) {
                select_monster = index;
                return;
            }
        }
    }

    // Enemyに対するマウスクリックの処理
    public int ClickEnemy(Vector3 mousePos) 
    {
        int index = -1;
        foreach(RawImage enemy_image in enemy_images) {
            index += 1;
            Vector3 relativeMousePos = enemy_image.transform.InverseTransformPoint(mousePos);
            Vector2 size = enemy_image.rectTransform.sizeDelta;
            if ((relativeMousePos.x >= -(size.x / 2) && relativeMousePos.x <= size.x / 2) &&
            (relativeMousePos.y >= -(size.y / 2) && relativeMousePos.y <= size.y / 2)
            ) {
                return index;
            }
        }
        return -1;
    }

    /* 最初のバトル開始時の処理
    * Monsterの決定、コマンドの配置を行う
    */
    public void StartBattle()
    {
        SetPlayerMonsters();

        // COMMAND BLOCKのセット処理
        GameObject command_prefab = Resources.Load<GameObject>("Command");
        for (int i = 0; i < 4; i++) {
            GameObject command_block = Instantiate(command_prefab);
            command_block.transform.SetParent(command_window.transform);
            command_blocks.Add(command_block);
        }
        Vector3[] command_block_pos = {new Vector3(-160, 35, 0), new Vector3(-160, -35, 0),
                                       new Vector3(160, 35, 0), new Vector3(160, -35, 0)};
        

        for (int i = 0; i < 4; i++) {
            command_blocks[i].transform.localPosition = command_block_pos[i];
            command_blocks[i].transform.Find("CommandText").GetComponent<TextMeshProUGUI>().text = command_block_str[i];
        }
        
        // set Monsters
        //無駄な処理多いからforループにしたいね
        num_monster = Random.Range(4,4);

        GameObject enemy_image_prefab = Resources.Load<GameObject>("EnemyImage");
        for (int i = 0; i < num_monster; i++) {
            GameObject enemy_image = Instantiate(enemy_image_prefab);
            enemy_image.transform.SetParent(Screen.transform);
            enemy_images.Add(enemy_image.GetComponent<RawImage>());
        }

        // 配置座標パターン
        // これを計算で出せるようにしないとswitch文が必要になる
        Vector3[] pos_pattern1 = {new Vector3(0,0,0)};
        Vector3[] pos_pattern2 = {new Vector3(-80,0,0), new Vector3(80,0,0)};
        Vector3[] pos_pattern3 = {new Vector3(-120,0,0), new Vector3(0,0,0), new Vector3(120,0,0)};
        Vector3[] pos_pattern4 = {new Vector3(-240,0,0), new Vector3(-80,0,0), new Vector3(80,0,0), new Vector3(240,0,0)};
        Monster monster_tmp;
        switch (num_monster) {
            case 1:
                ChooseMonsterByWeight();
                monster_tmp = new Monster(monster_param);
                enemy_images[0].enabled = true;
                enemy_images[0].texture = Resources.Load<Texture2D> (monster_param.image_path);
                enemy_images[0].transform.localPosition = pos_pattern1[0];
                monster_tmp.SetImage(enemy_images[0].GetComponent<RawImage>());
                monster_tmp.isEnemy = true;
                enemy_monsters.Add(monster_tmp);
                break;
            case 2:
                for (int i=0; i<2; i++) {
                    ChooseMonsterByWeight();
                    monster_tmp = new Monster(monster_param);
                    enemy_images[i].enabled= true;
                    enemy_images[i].texture = Resources.Load<Texture2D> (monster_param.image_path);
                    enemy_images[i].transform.localPosition = pos_pattern2[i];
                    monster_tmp.SetImage(enemy_images[i].GetComponent<RawImage>());
                    monster_tmp.isEnemy = true;
                    enemy_monsters.Add(monster_tmp);
                }
                break;
            case 3:
                for (int i=0; i<3; i++) {
                    ChooseMonsterByWeight();
                    monster_tmp = new Monster(monster_param);
                    enemy_images[i].enabled= true;
                    enemy_images[i].texture = Resources.Load<Texture2D> (monster_param.image_path);
                    enemy_images[i].transform.localPosition = pos_pattern3[i];
                    monster_tmp.SetImage(enemy_images[i].GetComponent<RawImage>());
                    monster_tmp.isEnemy = true;
                    enemy_monsters.Add(monster_tmp);
                }
                break;
            case 4:
                for (int i=0; i<4; i++) {
                    ChooseMonsterByWeight();
                    monster_tmp = new Monster(monster_param);
                    enemy_images[i].enabled= true;
                    enemy_images[i].texture = Resources.Load<Texture2D> (monster_param.image_path);
                    enemy_images[i].transform.localPosition = pos_pattern4[i];
                    monster_tmp.SetImage(enemy_images[i].GetComponent<RawImage>());
                    monster_tmp.isEnemy = true;
                    enemy_monsters.Add(monster_tmp);
                }
                break;
        }
        Destroy(start_button);
    }


    public List<Action> SetActionOrder() 
    {
        List<Action> _action_order = new List<Action>();
        for (int i = 0; i < num_pMonster; i++) {
            _action_order.Add(pMonsters[i].GetAction());
        }
        for (int i=0; i<num_monster; i++) {
            _action_order.Add(enemy_monsters[i].GetAction());
        }
        return _action_order;
    }

    public void SetActions()
    {
        for (int i = 0; i < num_pMonster; i++) {
            pMonsters[i].SetAction(new PlayerAction(pMonsters[i], select_monsters[i], 0));
        }
        for (int i=0; i<num_monster; i++) {
            int r = Random.Range(0, num_pMonster-1);
            enemy_monsters[i].SetAction(new EnemyAction(enemy_monsters[i], pMonsters[r], 0));
        }
    }

    void Update() 
    {
        Vector3 mousePos;
        switch (SceneMode) {
            case SceneType.SELECT:
                if (Input.GetMouseButtonDown(0)) {
                    mousePos = Input.mousePosition;
                    int command_id = ClickCommandBlock(mousePos);
                    if (command_id >= 0) {
                        Debug.Log($"block {command_id} is clicked");
                        HandleCommandSelect(command_id);
                    } else {
                        Debug.Log("どれもクリックされてない");
                    }
                }
                break;
            case SceneType.ATTACK:
                mousePos = Input.mousePosition;

                // 選択されているモンスター以外のカーソルを非表示
                for (int i = 0; i < num_monster; i++) {
                    if (i != select_monster) {
                        enemy_images[i].transform.Find("Cursor").gameObject.SetActive(false);
                    }
                }

                // カーソルの点滅処理
                _time += Time.deltaTime;
                var repeatValue = Mathf.Repeat((float)_time, cursor_blink_rate);
                enemy_images[select_monster].transform.Find("Cursor").gameObject.SetActive(repeatValue >= cursor_blink_rate/2);

                // クリック時の処理
                if (Input.GetMouseButtonDown(0)) {
                    mousePos = Input.mousePosition;
                    // ATTACKをやめるときの処理
                    if (ClickCommandBlock(mousePos) == 0) {

                        // 遷移処理
                        for (int i = 0; i < 4; i++) {
                            command_blocks[i].SetActive(true);
                        }
                        for (int i = 0; i < num_monster; i++) {
                            enemy_images[i].transform.Find("Cursor").gameObject.SetActive(false);
                        }
                        SceneMode = SceneType.SELECT;
                        break;
                    } else {
                        int enemy_id = ClickEnemy(mousePos);
                        if (enemy_id >= 0) {
                            selecter++;
                            // if (enemy_id != select_monster) {
                            //     Debug.Log("Error: click place is different from cursor place!");
                            // }
                            // 遷移処理

                            // 行動順序とそれぞれの行動を決定
                            select_monsters.Add(enemy_monsters[enemy_id]);

                            // 全員の行動が決定したとき
                            if (selecter >= num_pMonster) {
                                selecter = 0;
                                command_blocks[0].SetActive(false);
                                for (int i = 0; i < num_monster; i++) {
                                    enemy_images[i].transform.Find("Cursor").gameObject.SetActive(false);
                                }
                                SetActions();
                                action_order = SetActionOrder();
                                done_actions = new List<Action>();
                                scene_step = 0;
                                turn_wait_time = 1.0f;
                                Debug.Log(action_order[0].attacker.param.name_ja + "の攻撃");
                                SceneMode = SceneType.PROCESS;
                                break;
                            // まだ全員の行動が決まっていないとき
                            } else {
                                for (int i = 0; i < 4; i++) {
                                    command_blocks[i].SetActive(true);
                                }
                                for (int i = 0; i < num_monster; i++) {
                                    enemy_images[i].transform.Find("Cursor").gameObject.SetActive(false);
                                }
                                SceneMode = SceneType.SELECT;
                                break;
                            }
                        }
                    }
                }
                FocusEnemy(mousePos);
                break;
            case SceneType.PROCESS:
                turn_wait_time -= Time.deltaTime;
                switch (scene_step) {
                    // 行動者宣言
                    case 0:
                        if (!(turn_wait_time > 0)) {
                            scene_step++;
                            turn_wait_time = 1.0f;
                            action_order[0].HandleAction();
                            Debug.Log(action_order[0].attacker.param.name_ja+"から"+action_order[0].defender.param.name_ja +"への攻撃");
                            UpdateStatusWindow();
                        }  
                        break;
                    // アクション後処理
                    case 1:
                        // 被ダメージ者の点滅処理
                        damage_blink_time += Time.deltaTime;
                        var damageBlinkRepeatValue = Mathf.Repeat((float)damage_blink_time, damage_blink_rate);
                        action_order[0].defender.GetImage().enabled = damageBlinkRepeatValue >= damage_blink_rate/2;

                        if (!(turn_wait_time > 0)) {
                            action_order[0].defender.GetImage().enabled = true;
                            scene_step++;
                            turn_wait_time = 1.0f;

                            // 攻撃対象の敵が死んでいたとき
                            if (action_order[0].defender.isDead()) {
                                // 敵だった場合
                                if (action_order[0].defender.isEnemy) {
                                    num_monster -= 1;
                                    action_order[0].defender.GetImage().gameObject.SetActive(false);
                                    enemy_monsters.Remove(action_order[0].defender);
                                    enemy_images.Remove(action_order[0].defender.GetImage());
                                //味方だった場合
                                } else {
                                    num_pMonster -= 1;
                                    pMonsters.Remove(action_order[0].defender);
                                    UpdateStatusWindowDead(action_order[0].defender);
                                }
                                action_order.Remove(action_order[0].defender.GetAction());
                                // このターン以降の攻撃対象の更新処理
                                for (int i = 1; i < action_order.Count; i++) {
                                    if (action_order[i].defender == action_order[0].defender) {
                                        if (!(action_order[i].attacker.isEnemy)) {
                                            if (enemy_monsters.Count != 0){
                                                action_order[i].defender = enemy_monsters[0];
                                            }
                                        } else {
                                            if (pMonsters.Count != 0) {
                                                action_order[i].defender = pMonsters[0];
                                            }
                                        }
                                    }
                                }
                            }
                        }  
                        break;
                    case 2:
                        if (!(turn_wait_time > 0)) {
                            done_actions.Add(action_order[0]);
                            action_order.RemoveAt(0);
                            scene_step = 0;
                            if (num_monster == 0) {
                                Debug.Log("勝利");
                                SceneMode = SceneType.END;
                            }else if (action_order.Count == 0) {
                                Debug.Log("ターン処理終了");
                                for (int i = 0; i < 4; i++) {
                                    command_blocks[i].SetActive(true);
                                }
                                SceneMode = SceneType.SELECT;
                                break;
                            }
                            Debug.Log(action_order[0].attacker.param.name_ja + "の攻撃");
                        }
                        break;
                }
                break;
            default:
                break;
        }
    }
}
