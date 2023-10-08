using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    private double _time;
    
    private GameObject Screen;
    private Canvas second_canvas;
    private SecondCanvasManager scm;
    private MonsterData monster_data;
    private SkillData skill_data;
    private TextMeshProUGUI battleMessage1;
    private TextMeshProUGUI battleMessage2;

    private List<RawImage> enemy_images = new List<RawImage>();

    // player's monster info
    private int[] pMonster_id_list = {3, 3, 3, 6};
    private int num_pMonster = 4;
    private List<Monster> pMonsters = new List<Monster>();
    private List<GameObject> status_windows = new List<GameObject>();

    // enemy monster info
    private List<Monster> enemy_monsters = new List<Monster>();
    private MonsterData.Param monster_param;
    private int num_monster;

    private GameObject command_window;
    private List<GameObject> command_blocks = new List<GameObject>();

    // シーンの制御をするための変数
    private SceneType SceneMode; 
    private bool isCalledOnce;

    // ATTACK時に使用する変数
    private int select_monster;
    private List<Skill> select_skills;
    private List<Monster> select_monsters;
    private float cursor_blink_rate = 0.5f;
    private int selecter = 0;

    // PROCESS時に使用する変数
    private int battle_scene_step = 0;
    private List<Action> action_order;
    private List<Action> done_actions;
    private float damage_blink_rate = 0.5f;
    private double damage_blink_time = 0;
    private double turn_wait_time = 0;

    // END時に使用する変数
    private int end_scene_step = 0;
    private double end_wait_time = 0;
    
    // 仮に使用
    private List<List<int>> pMonster_skill_ids;
    private Skill default_attack;


    public enum CommandType
    {
        ATTACK = 0,
        ITEM = 1,
        SPECIAL = 2,
        ESCAPE = 3
    }

    public enum SceneType
    {
        START,
        SELECT,
        ATTACK,
        ITEM,
        SPECIAL,
        ESCAPE,
        PROCESS,
        END,
        ANOTHER,
    }

    private string[] command_block_str;

    // Start is called before the first frame update
    void Start()
    {
        SceneMode = SceneType.START;
        command_window = GameObject.Find("CommandWindow");
        monster_data = Resources.Load("monster_data") as MonsterData;
        skill_data = Resources.Load("skill_data") as SkillData;
        Screen = GameObject.Find("BackgroundImage");
        second_canvas = Screen.transform.Find("SecondCanvas").GetComponent<Canvas>();
        scm = new SecondCanvasManager(second_canvas);
        battleMessage1 = GameObject.Find("BattleMessage1").GetComponent<TextMeshProUGUI>();
        battleMessage2 = GameObject.Find("BattleMessage2").GetComponent<TextMeshProUGUI>();
        battleMessage1.enabled = false;
        battleMessage2.enabled = false;
        command_block_str = new string[]{"こうげき", "どうぐ", "とくぎ", "にげる"};

        // 仮の形式
        pMonster_skill_ids = new List<List<int>>();
        List<int> pMonster1_skill_ids = new List<int>(){1,2,3};
        pMonster_skill_ids.Add(pMonster1_skill_ids);
        List<int> pMonster2_skill_ids = new List<int>(){1,1,1,1,1,1,1,1,1,2,3,4,5,6,7,8,9,10,11,12,13};
        pMonster_skill_ids.Add(pMonster2_skill_ids);
        List<int> pMonster3_skill_ids = new List<int>(){1,2,5,6,7};
        pMonster_skill_ids.Add(pMonster3_skill_ids);
        List<int> pMonster4_skill_ids = new List<int>(){8,9,13};
        pMonster_skill_ids.Add(pMonster4_skill_ids);
        
        default_attack = new Skill(skill_data.sheets[0].list[0]);
        
        isCalledOnce = true;
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
            monster.SetSkills(pMonster_skill_ids[i], skill_data);
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
            status_windows[i].transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = pMonsters[i].param.name_ja;
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
            if (!command_block.activeSelf) continue;
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

        Vector3 sw_pos;
        switch (command_id) {
            case 0:
                SceneMode = SceneType.ATTACK;
                sw_pos = pMonsters[selecter].GetStatusWindow().transform.position;
                sw_pos.y -= 5;
                pMonsters[selecter].GetStatusWindow().transform.position = sw_pos;
                if (selecter == 0) {
                    select_monsters = new List<Monster>();
                    select_skills = new List<Skill>();
                }
                select_skills.Add(default_attack);
                select_monster = 0;
                break;
            case 1:
                SceneMode = SceneType.ITEM;
                break;
            case 2:
                sw_pos = pMonsters[selecter].GetStatusWindow().transform.position;
                sw_pos.y -= 5;
                pMonsters[selecter].GetStatusWindow().transform.position = sw_pos;
                SceneMode = SceneType.SPECIAL;
                scm.OpenSkillWindow(pMonsters[selecter]);
                if (selecter == 0) {
                    select_monsters = new List<Monster>();
                    select_skills = new List<Skill>();
                }
                select_monster = 0;
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
            Debug.Log(command_block_str[i]);
            command_blocks[i].transform.Find("CommandText").GetComponent<TextMeshProUGUI>().text = command_block_str[i];
            command_blocks[i].gameObject.SetActive(false);
        }
        
        // set Monsters
        //無駄な処理多いからforループにしたいね
        num_monster = Random.Range(1,4);

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
        selecter = 0;
    }

    public List<Action> SetActionOrder() 
    {
        List <Monster> all_monsters = new List<Monster>();
        for (int i = 0; i < pMonsters.Count; i++) {
            all_monsters.Add(pMonsters[i]);
        }
        for (int i = 0; i < enemy_monsters.Count; i++) {
            all_monsters.Add(enemy_monsters[i]);
        }
        all_monsters.Sort((m1,m2) => m2.param.sp - m1.param.sp);
        List<Action> _action_order = new List<Action>();
        for (int i = 0; i < all_monsters.Count; i++) {
            _action_order.Add(all_monsters[i].GetAction());
        }
        return _action_order;
    }

    public void SetActions()
    {
        for (int i = 0; i < num_pMonster; i++) {
            pMonsters[i].SetAction(new PlayerAction(pMonsters[i], select_monsters[i], select_skills[i]));
        }
        for (int i=0; i<num_monster; i++) {
            int r = Random.Range(0, num_pMonster-1);
            enemy_monsters[i].SetAction(new EnemyAction(enemy_monsters[i], pMonsters[r], default_attack));
        }
    }

    private IEnumerator StartProcess() {
        StartBattle();
        string startMessage;
        if (num_monster == 1) {
            startMessage = enemy_monsters[0].param.name_ja + "が現れた！";
        } else if (num_monster > 1) {
            startMessage = enemy_monsters[0].param.name_ja + "たちが現れた！";
        } else {
            startMessage = "";
            Debug.Log("Error: モンスター出現エラー");
        }
        battleMessage1.text = "";
        battleMessage1.enabled = true;
        battleMessage1.text += startMessage[0];
        for (int i = 1; i < startMessage.Length; i++) {
            battleMessage1.text += startMessage[i];
            yield return new WaitForSeconds (0.1f);

        }
        yield return new WaitForSeconds (1.5f);
        battleMessage1.enabled = false;
        SceneMode = SceneType.SELECT;
        for (int i = 0; i < 4; i++) {
            command_blocks[i].SetActive(true);
        }
    }

    void Update() 
    {
        Vector3 mousePos;
        Vector3 sw_pos;
        switch (SceneMode) {
            case SceneType.START:
                if (isCalledOnce){
                    StartCoroutine("StartProcess");
                    isCalledOnce = false;
                }
                break;
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
                    if (ClickCommandBlock(mousePos) >= 0) {
                        sw_pos = pMonsters[selecter].GetStatusWindow().transform.position;
                        sw_pos.y += 5;
                        pMonsters[selecter].GetStatusWindow().transform.position = sw_pos;
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
                            sw_pos = pMonsters[selecter].GetStatusWindow().transform.position;
                            sw_pos.y += 5;
                            pMonsters[selecter].GetStatusWindow().transform.position = sw_pos;
                            selecter++;
                            // 遷移処理
                            // 行動順序とそれぞれの行動を決定
                            select_monsters.Add(enemy_monsters[enemy_id]);

                            // 全員の行動が決定したとき
                            if (selecter >= num_pMonster) {
                                selecter = 0;
                                for (int i = 0; i < 4; i++) command_blocks[i].SetActive(false);

                                for (int i = 0; i < num_monster; i++) {
                                    enemy_images[i].transform.Find("Cursor").gameObject.SetActive(false);
                                }
                                SetActions();
                                action_order = SetActionOrder();
                                done_actions = new List<Action>();
                                battle_scene_step = 0;
                                battleMessage1.enabled = true;
                                turn_wait_time = 1.0f;
                                Debug.Log(action_order[0].attacker.param.name_ja + "の" + action_order[0].skill.param.name_ja);
                                battleMessage1.text = action_order[0].attacker.param.name_ja  + "の" + action_order[0].skill.param.name_ja + "！";
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
                switch (battle_scene_step) {
                    // 行動者宣言
                    case 0:
                        if (!(turn_wait_time > 0)) {
                            battle_scene_step++;
                            turn_wait_time = 1.0f;
                            action_order[0].HandleAction(battleMessage2);
                            // battleMessage2.text = action_order[0].defender.param.name_ja + "は" + 
                            // action_order[0].attacker.param.atk.ToString() + "ダメージを受けた！";
                            battleMessage2.enabled = true;
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
                            battle_scene_step++;
                            battleMessage1.enabled = false;
                            battleMessage2.enabled = false;

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
                            battle_scene_step = 0;

                            // 敵を全滅させたとき
                            if (num_monster == 0) {
                                battleMessage1.text = "戦いに勝利した！";
                                battleMessage1.enabled = true;
                                SceneMode = SceneType.END;
                                end_scene_step = 0;
                                break;
                            }
                            // 味方が全滅したとき
                            else if (num_pMonster == 0) {
                                battleMessage1.text = "全滅してしまった！";
                                battleMessage1.enabled = true;
                                SceneMode = SceneType.END;
                                end_scene_step = 0;
                                break;
                            }
                            // アクションが全て終わったとき
                            else if (action_order.Count == 0) {
                                Debug.Log("ターン処理終了");
                                for (int i = 0; i < 4; i++) {
                                    command_blocks[i].SetActive(true);
                                }
                                SceneMode = SceneType.SELECT;
                                break;
                            } 
                            else {
                                Debug.Log(action_order[0].attacker.param.name_ja + "の攻撃");
                                battleMessage1.text = action_order[0].attacker.param.name_ja + "の" +
                                action_order[0].skill.param.name_ja + "！";
                                battleMessage1.enabled = true;
                                turn_wait_time = 1.0f;
                            }
                        }
                        break;
                }
                break;
            case SceneType.END:
                end_wait_time -= Time.deltaTime;
                switch (end_scene_step) {
                    // battle endに突入
                    case 0:
                        end_scene_step++;
                        end_wait_time = 1.0f;
                        break;
                    case 1:
                        if (!(end_wait_time > 0)) {
                            battleMessage1.enabled = false;
                            // scene遷移
                            SceneMode = SceneType.ANOTHER;
                        }
                        break;
                }
                break;
            
            case SceneType.ITEM:
                if (Input.GetMouseButtonDown(0)) {
                    mousePos = Input.mousePosition;
                    // ITEMをやめるときの処理
                    if (ClickCommandBlock(mousePos) >= 0) {
                        // 遷移処理
                        for (int i = 0; i < 4; i++) {
                            command_blocks[i].SetActive(true);
                        }
                        SceneMode = SceneType.SELECT;
                        break;
                    }
                }
                break;
            case SceneType.SPECIAL:
                if (Input.GetMouseButtonDown(0)) {
                    mousePos = Input.mousePosition;
                    // SPECIALをやめるときの処理
                    scm.NextSkillWindow(mousePos, pMonsters[selecter]);
                    scm.BackSkillWindow(mousePos, pMonsters[selecter]);
                    if(scm.CloseWindow(mousePos)) {
                        for (int i = 0; i < 4; i++) {
                            command_blocks[i].SetActive(true);
                        }
                        sw_pos = pMonsters[selecter].GetStatusWindow().transform.position;
                        sw_pos.y += 5;
                        pMonsters[selecter].GetStatusWindow().transform.position = sw_pos;
                        SceneMode = SceneType.SELECT;
                        break;
                    }

                    // skillが選択されたとき
                    int select_skill = scm.SelectSkills(mousePos);
                    if (select_skill >= 0) {
                        Debug.Log(select_skill);
                        SceneMode = SceneType.ATTACK;
                        select_skills.Add(pMonsters[selecter].skills[select_skill]);
                        Debug.Log(select_skills[selecter]);
                        break;
                    }
                }
                break;
            case SceneType.ESCAPE:
            if (Input.GetMouseButtonDown(0)) {
                    mousePos = Input.mousePosition;
                    // ESCAPEをやめるときの処理
                    if (ClickCommandBlock(mousePos) >= 0) {
                        // 遷移処理
                        for (int i = 0; i < 4; i++) {
                            command_blocks[i].SetActive(true);
                        }
                        SceneMode = SceneType.SELECT;
                        break;
                    }
                }
                break;
            default:
                break;
        }
    }
}

