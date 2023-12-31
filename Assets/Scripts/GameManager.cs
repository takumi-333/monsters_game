using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private Canvas command_canvas;
    private CommandWindowManager CWM;

    private Canvas enemy_canvas;
    private Canvas status_window_canvas;
    private MonsterManager MM;

    private BattleManager BM;

    private Canvas second_canvas;
    private SecondCanvasManager SCM;

    private Canvas side_command_canvas;
    private SideCommandManager SComM;

    public List<PlayerMonster> player_monsters;
    public List<EnemyMonster> enemy_monsters;

    private bool focus_flg;
    private MonsterData monster_data;
    private SkillData skill_data;

    private float cursor_blink_rate;
    private float wait_time;
    
    private SceneType scene_type;
    private bool started;

    private Skill default_attack;
    private EndingType ending_type;

    private AudioSource audio_source;
    public AudioClip win_sound;
    public AudioClip lose_sound;
    public AudioClip escape_sound;
    public AudioClip boss_bgm;

    private bool isCalledEnding;
    private bool levelUp;
    private bool add_monster;

    // MAPに戻る時に必要な情報
    public string map_scene_name;
    public Vector3 player_position;

    public int event2_flg;
    public bool lose_event = false;
    public bool boss_battle = false;

    public int max_num_monsters;

    void Start()
    {
        // コンポーネントの取得
        audio_source = GetComponent<AudioSource>();
        if (boss_battle) {
            audio_source.clip = boss_bgm;
        }
        audio_source.Play();
        scene_type = SceneType.START;
        command_canvas = GameObject.FindWithTag("CommandCanvas").GetComponent<Canvas>();
        enemy_canvas = GameObject.FindWithTag("EnemyCanvas").GetComponent<Canvas>();
        second_canvas = GameObject.FindWithTag("SecondCanvas").GetComponent<Canvas>();
        status_window_canvas = GameObject.FindWithTag("StatusWindowCanvas").GetComponent<Canvas>();
        side_command_canvas = GameObject.FindWithTag("SideCommandCanvas").GetComponent<Canvas>();

        // 各Managerのセット
        CWM = new CommandWindowManager(command_canvas);
        MM = new MonsterManager(status_window_canvas, enemy_canvas, map_scene_name, max_num_monsters);
        SCM = new SecondCanvasManager(second_canvas);
        SComM = new SideCommandManager(side_command_canvas);
        
        BM = new BattleManager(MM);

        skill_data = Resources.Load("skill_data") as SkillData;
        default_attack = new Skill(skill_data.sheets[0].list[0]);

        // debug用
        monster_data = Resources.Load("monster_data") as MonsterData;
        Skill attack1 = new Skill(skill_data.sheets[0].list[1]);
        Skill attack2 = new Skill(skill_data.sheets[0].list[2]);
        Skill attack3 = new Skill(skill_data.sheets[0].list[3]);
        
        MM.SetPlayerMonsters(player_monsters);
        if (enemy_monsters == null) {
            enemy_monsters = MM.enemy_monsters;
        }
        else {
            MM.enemy_monsters = enemy_monsters;
        }
        MM.SetEnemyMonsters();
        foreach(EnemyMonster emon in MM.enemy_monsters) {
            emon.AddSkill(default_attack);
        }

        wait_time = 0;
        cursor_blink_rate = 0.3f;
        started = false;
        isCalledEnding = false;
        levelUp = false;
        add_monster = false;
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
        TAME,
        END,
    }

    public enum EndingType
    {
        WIN,
        LOSE,
        ESCAPE,
    }

    private void StartProcess()
    {
        string start_message = BM.GetStartMessage();
        StartCoroutine(CWM.DisplayMessage1(start_message));
    }

    public IEnumerator ProcessBattle()
    {
        if (BM.processing_battle) yield break;
        BM.processing_battle = true;
        while (BM.action_order.Count > 0) {

            // 計算処理
            Action action = BM.action_order[0];
            string bm1 = action.attacker.name_ja + "の" + action.skill.param.name_ja + "！";
            CWM.SetBattleMessage1(bm1);
            int total_damage = action.HandleAction();
            if (!action.attacker.isEnemy) MM.UpDownPlayerMonsterWindow((PlayerMonster)action.attacker, 2.5f/SComM.battle_speed);
            yield return new WaitForSeconds(1.5f/SComM.battle_speed);
            
            // 点滅・ステータス更新処理
            string bm2 = action.defender.name_ja +"は" + total_damage + "ダメージを受けた！";
            CWM.SetBattleMessage2(bm2);
            StartCoroutine(MM.BlinkMonster(action.defender, 0.2f/SComM.battle_speed));
            MM.UpdateStatusWindow();
            
            yield return new WaitForSeconds(1.5f/SComM.battle_speed);
            // 死んでいた場合の処理
            action.defender.CheckDead();
            action.attacker.CheckDead();
            if (action.defender.isDead) {
                if (action.defender.isEnemy) {
                    action.defender.GetImage().enabled = false;
                    MM.dead_enemy_monsters.Add((EnemyMonster)action.defender);
                }
                BM.ChangeActionTarget(action.defender);
            }
            yield return new WaitForSeconds(0.5f/SComM.battle_speed);
            if (action.attacker.isDead) BM.ChangeActionTarget(action.attacker);

            // 後処理
            CWM.ClearAllMessage();
            BM.action_order.Remove(action);
            BM.done_actions.Add(action);
        }

        if (BM.CheckBattleWin()) {
            ending_type = EndingType.WIN;
            scene_type = SceneType.END;
        } else if (BM.CheckBattleLose()) {
            ending_type = EndingType.LOSE;
            scene_type = SceneType.END;
        } else {
            // select処理へ
            BM.StartTurn();
            CWM.SetCommandsActive(true);
            scene_type = SceneType.SELECT;
        }
        BM.processing_battle = false;
    }

    

    private void HandleCommandSelect(int command_id)
    {
        // 押されたコマンドブロック以外のブロックを非表示
        CWM.SelectedCommandActive(command_id);
        switch (command_id) {
            case 0:
                scene_type = SceneType.ATTACK;
                if (BM.selecting_monsters.Count > 0) {
                    BM.selecter_monster = BM.selecting_monsters[0];
                    MM.DownPlayerMonsterWindow(BM.selecter_monster);
                    BM.selecter_monster.SetAction(new Action(BM.selecter_monster, null, default_attack));
                } else {
                    Debug.LogError("Error: cannot detect who's turn");
                }
                break;
            case 1:
                scene_type = SceneType.ITEM;
                break;
            case 2:
                scene_type = SceneType.SPECIAL;

                // 選択者がいれば、その選択者のステータスウィンドウを下げ、スキルウィンドウを開く
                if (BM.selecting_monsters.Count > 0) {
                    BM.selecter_monster = BM.selecting_monsters[0];
                    MM.DownPlayerMonsterWindow(BM.selecter_monster);
                    SCM.OpenSkillWindow(BM.selecter_monster);
                } else {
                    Debug.LogError("Error: cannot detect who's turn");
                }
                break;
            case 3:
                scene_type = SceneType.ESCAPE;
                break;
            default:
                break;
        }
    }

    private void CancelCommandSelect()
    {
        switch (scene_type) {
            case SceneType.ATTACK:
                MM.UpPlayerMonsterWindow(BM.selecter_monster);
                MM.ClearCursors();
                BM.selecter_monster = null;
                break;
            case SceneType.SPECIAL:
                break;
            case SceneType.ITEM:
                break;
            case SceneType.ESCAPE:
                break;
        }
        CWM.SetCommandsActive(true);
        scene_type = SceneType.SELECT;
    }

    // ATTACKのとき、敵を選択した際の処理
    private void HandleEnemySelect(EnemyMonster enemy_monster)
    {
        BM.HandleEnemySelect(enemy_monster);

        // まだ選び終えていないキャラがいる
        if (BM.selecting_monsters.Count > 0) {
            scene_type = SceneType.SELECT;
            CWM.SetCommandsActive(true);

        // 全員選択を終えた際の処理
        }else {
            CWM.SetCommandsActive(false);
            BM.SetActionOrder();
            scene_type = SceneType.PROCESS;
        }
    }

    private void CallMapScene()
    {
            // イベントにメソッドを登録
            SceneManager.sceneLoaded += GameSceneLoaded;

            SceneManager.LoadScene(map_scene_name);

        void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            // シーン切り替え後のスクリプトを取得
            var gameManager = 
                GameObject.FindWithTag("MapManager").GetComponent<MapManager>();

            // データを渡す処理
            gameManager.player_monsters = player_monsters;
            gameManager.player_position = player_position;
            Debug.Log(event2_flg);
            gameManager.event2_flg = event2_flg;

            // イベントからメソッドを削除
            SceneManager.sceneLoaded -= GameSceneLoaded;
        }
    }

    public IEnumerator LevelUpMessage() 
    {
        levelUp = true;
        List<TextMeshProUGUI> level_up_texts = new List<TextMeshProUGUI>();
        foreach(PlayerMonster player_monster in MM.player_monsters) {
            if (player_monster.level_up) {
                TextMeshProUGUI text = player_monster.GetStatusWindow().transform.Find("LevelUpText").GetComponent<TextMeshProUGUI>();
                text.gameObject.SetActive(true);
                level_up_texts.Add(text);
            }
        }
        if (level_up_texts.Count == 0) {
            levelUp = false;
            yield break;
        }
        string level_up_message = "レベルアップ！";
        int i = 0;
        // 初期化
        foreach (TextMeshProUGUI level_up_text in level_up_texts) {
            level_up_text.text = "";
        }
        while(i < level_up_message.Length) {
            foreach (TextMeshProUGUI level_up_text in level_up_texts) {
                level_up_text.text += level_up_message[i];
            }
            i++;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.75f);
        foreach (TextMeshProUGUI level_up_text in level_up_texts) {
            level_up_text.enabled = false;
        }
        levelUp = false;
    }

    private IEnumerator FinishBattle()
    {
        if (isCalledEnding) yield break;
        MM.ReviveMonsters();
        isCalledEnding = true;
        audio_source.Stop();
        switch (ending_type) {
            case EndingType.WIN:
                audio_source.PlayOneShot(win_sound);
                CWM.SetBattleMessage1("戦いに勝利した!");
                Debug.Log("戦いに勝利した表示");
                MM.HandleExpProcess();
                StartCoroutine("LevelUpMessage");
                yield return new WaitForSeconds(0.5f);
                yield return new WaitUntil(() => !levelUp);
                CWM.ClearAllMessage();
                EnemyMonster new_monster = BM.CheckCanAddMonster();
                // 仲間にすることができる
                if (new_monster != null) {
                    Debug.Log("monsterが起き上がった！");
                    new_monster.GetImage().enabled = true;
                    CWM.SetBattleMessage1(new_monster.name_ja + "を仲間にできます");
                    CWM.DisplayTameMessage();
                    CWM.taming = true;
                    scene_type = SceneType.TAME;
                    yield return new WaitWhile(() => CWM.taming);
                    // 「仲間にする」を選択
                    if (add_monster) {
                        PlayerMonster new_player_monster = MM.ChangeToPlayerMonster(new_monster);
                        if (player_monsters.Count < 4) {
                            player_monsters.Add(new_player_monster);
                        } else {
                            FarmDataManager FDM = new FarmDataManager();
                            FarmData farm_data = FDM.Load();
                            if (farm_data == null) {
                                farm_data = new FarmData();
                            }
                            farm_data.AddMonsterData(new_player_monster);
                            FDM.Save(farm_data);

                            SaveDataManager SDM = new SaveDataManager();
                            SaveMonsterData save_data = new SaveMonsterData(player_monsters);
                            save_data.SetMonsterData(player_monsters);
                            SDM.Save(save_data);
                        }
                    }
                    new_monster = null;
                }
                break;
            case EndingType.LOSE:
                CWM.SetBattleMessage1("全滅してしまった...");
                audio_source.PlayOneShot(lose_sound);
                yield return new WaitWhile(() => audio_source.isPlaying);
                yield return new WaitForSeconds(0.2f);
                if (!lose_event) {
                    SceneManager.LoadScene("TitleScene");
                    yield break;
                }
                break;
            case EndingType.ESCAPE:
                CWM.SetBattleMessage1("逃走した！");
                audio_source.PlayOneShot(escape_sound);
                break;
        }
        yield return new WaitWhile(() => audio_source.isPlaying);
        yield return new WaitForSeconds(0.2f);
        // battleMessage1.enabled = false;
        CallMapScene();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos;
        if (Input.GetMouseButtonDown(0)) {
            mousePos = Input.mousePosition;
            SComM.HandleClickDoubleSpeeder(mousePos);
        }
        switch (scene_type) {
            case SceneType.START:
                if (!started) {
                    string start_message = BM.GetStartMessage();
                    StartCoroutine(CWM.DisplayMessage1(start_message));
                    started = true;
                }
                if (!CWM.displaying && started) {
                    scene_type = SceneType.SELECT;
                    BM.StartTurn();
                    CWM.SetCommandsActive(true);
                    SComM.SetSpeederActive(true);
                }
                break;
            case SceneType.SELECT:
                if (Input.GetMouseButtonDown(0)) 
                {
                    mousePos = Input.mousePosition;
                    int command_id = CWM.HandleClickCommand(mousePos);
                    if (command_id >= 0) HandleCommandSelect(command_id);
                }
                break;
            case SceneType.ATTACK:
                wait_time -= Time.deltaTime;

                // cursor点滅処理
                if (wait_time <= 0){
                    wait_time = cursor_blink_rate/ SComM.battle_speed;
                    mousePos = Input.mousePosition;
                    MM.FocusEnemy(mousePos);
                }

                // マウスをクリックしたとき
                if (Input.GetMouseButtonDown(0))
                {
                    mousePos = Input.mousePosition;
                    EnemyMonster enemy_monster = MM.ClickEnemy(mousePos);
                    // commandがクリックされた場合
                    if (CWM.HandleClickCommand(mousePos) >= 0) {
                        CancelCommandSelect();
                    } else if (enemy_monster != null) {
                        HandleEnemySelect(enemy_monster);
                    }
                    
                }
                break;
            case SceneType.SPECIAL:
                if (Input.GetMouseButtonDown(0))
                    {
                        mousePos = Input.mousePosition;
                        // commandがクリックされた場合
                        SCM.NextSkillWindow(mousePos, BM.selecter_monster);
                        SCM.BackSkillWindow(mousePos, BM.selecter_monster);
                        if(SCM.CloseWindow(mousePos)) {
                            CancelCommandSelect();
                            break;
                        }
                        Skill select_skill = SCM.SelectSkill(mousePos, BM.selecter_monster);
                        
                        // スキルが選択されたときの処理
                        if (select_skill != null) {
                            scene_type = SceneType.ATTACK;
                            BM.selecter_monster.SetAction(new Action(BM.selecter_monster, null, select_skill));
                        }
                    }
                break;
            case SceneType.ITEM:
                if (Input.GetMouseButtonDown(0))
                    {
                        mousePos = Input.mousePosition;
                        // commandがクリックされた場合
                        if (CWM.HandleClickCommand(mousePos) >= 0) {
                            CancelCommandSelect();
                        }
                    }
                break;
            case SceneType.ESCAPE:
                ending_type = EndingType.ESCAPE;
                scene_type = SceneType.END;
                CWM.SetCommandsActive(false);
                break;
            case SceneType.PROCESS:
                StartCoroutine("ProcessBattle");
                break;
            case SceneType.END:
                StartCoroutine("FinishBattle");
                break;
                
            case SceneType.TAME:
                if (CWM.taming) {

                    // 仲間にするcursorの点滅処理
                    wait_time -= Time.deltaTime;
                    if (wait_time <= 0){
                        wait_time = cursor_blink_rate/ SComM.battle_speed;
                        if (CWM.focus_cursor == null) {
                            foreach(Image cursor in CWM.tame_cursors) {
                                cursor.enabled = false;
                            }
                        }
                        if (CWM.focus_cursor != null) {
                            foreach(Image cursor in CWM.tame_cursors) {
                                if (cursor != CWM.focus_cursor) {
                                    cursor.enabled = false;
                                } else {
                                    cursor.enabled = !cursor.enabled;
                                }
                            }
                        }
                    }
                    mousePos = Input.mousePosition;
                    CWM.SelectTameMessage(mousePos);
                    if (Input.GetMouseButtonDown(0) && CWM.focus_cursor != null) {
                        CWM.taming = false;

                        // この処理はCWMの関数として書くべき
                        if (CWM.focus_cursor == CWM.tame_cursors[0]) {
                            add_monster = true;
                        } else {
                            add_monster = false;
                        }
                    }
                    if (Input.GetKey(KeyCode.Y)) {
                        CWM.taming = false;
                        add_monster = true;
                    }
                    if (Input.GetKey(KeyCode.N)) {
                        CWM.taming = false;
                        add_monster = false;
                    }
                }
                break;
            default:
                break;
        }
    }
}
