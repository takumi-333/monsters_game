using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MonsterManager
{
    private string map_scene_name;

    public List<PlayerMonster> player_monsters;
    public List<EnemyMonster> enemy_monsters;
    // public List<PlayerMonster> dead_player_monsters;
    public List<EnemyMonster> dead_enemy_monsters;

    private MonsterData monster_data;
    private EachMonsterData each_monster_data;
    private SkillData skill_data;
    private MapMonsterData map_monster_data;

    private Canvas status_window_canvas;
    private Canvas enemy_canvas;

    // 味方のステータスウィンドウと敵の画像オブジェクト
    private List<GameObject> status_windows;
    private List<GameObject> enemy_objects;

    // モンスターの数
    public int num_player_monsters;
    public int num_enemy_monsters;
    public int num_dead_player_monsters;
    public int num_dead_enemy_monsters;

    public int max_num_monsters;
    private Color orange = new Color(238f/255f, 120f/255f, 0);



    public MonsterManager(Canvas status_window_canvas, Canvas enemy_canvas, string map_scene_name, int max_num_monsters)
    {
        this.status_window_canvas = status_window_canvas;
        this.enemy_canvas = enemy_canvas;
        this.map_scene_name = map_scene_name;
        this.max_num_monsters = max_num_monsters;

        player_monsters = new List<PlayerMonster>();
        enemy_monsters = new List<EnemyMonster>();
        // dead_player_monsters = new List<PlayerMonster>();
        dead_enemy_monsters = new List<EnemyMonster>();
        num_dead_player_monsters = 0;
        num_dead_enemy_monsters = 0;

        skill_data = Resources.Load("skill_data") as SkillData;
        map_monster_data = Resources.Load("map_monster_data") as MapMonsterData;
        each_monster_data = Resources.Load("each_monster_data") as EachMonsterData;
        monster_data = Resources.Load("monster_data") as MonsterData;
        

        status_windows = new List<GameObject>();
        enemy_objects = new List<GameObject>();

        for (int i = 0; i < 4; i++) {
            GameObject status_window = status_window_canvas.transform.Find($"StatusWindow{i}").gameObject;
            status_window.SetActive(false);
            status_windows.Add(status_window);
        }

        for (int i = 0; i < 4; i++) {
            GameObject enemy_object = enemy_canvas.transform.Find($"Enemy{i}").gameObject;
            enemy_object.SetActive(false);
            enemy_objects.Add(enemy_object);
        }
    }


    // プレイヤーのモンスターを受け取り、ステータスウィンドウに表示
    public void SetPlayerMonsters(List<PlayerMonster> player_monsters) 
    {
        num_player_monsters = player_monsters.Count;
        for (int i = 0; i < num_player_monsters; i++) {
            player_monsters[i].SetImage(status_windows[i].transform.Find("MonsterImage").GetComponent<RawImage>());
            player_monsters[i].SetStatusWindow(status_windows[i]);
            status_windows[i].SetActive(true);
            this.player_monsters.Add(player_monsters[i]);
        }

        for (int i = 0; i < num_player_monsters; i++) {
            status_windows[i].transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = player_monsters[i].name_ja;
            status_windows[i].transform.Find("MonsterImage").GetComponent<RawImage>().texture = Resources.Load<Texture2D>(player_monsters[i].image_path);
            UpdateStatusWindow();
        }
    }

    // 出現するモンスターを出現確率から決定
    public EnemyMonster GenerateMonsterByWeight() 
    {
        MapMonsterData.Sheet map_monster_data_sheet = map_monster_data.sheets.Find(sheet=>sheet.name==map_scene_name);
        int total_weight = 0;
        EachMonsterData.Param monster_param;
        EnemyMonster enemy_monster;
        for (int i = 0; i < map_monster_data_sheet.list.Count; i++) {
            total_weight += map_monster_data_sheet.list[i].weight;
        }
        int r = Random.Range(1, total_weight);
        for (int i = 0; i < map_monster_data_sheet.list.Count; i++) {
            if (r < map_monster_data_sheet.list[i].weight) {
                Debug.Log(map_scene_name);
                Debug.Log(map_monster_data_sheet.list[i].id);
                monster_param = each_monster_data.sheets.Find(sheet => sheet.name == map_monster_data_sheet.list[i].id.ToString()).list.Find(param=>param.lv == map_monster_data_sheet.list[i].level);
                enemy_monster = new EnemyMonster(monster_param, monster_data.sheets[0].list.Find(param=>param.id==map_monster_data_sheet.list[i].id));
                enemy_monster.friendly = map_monster_data_sheet.list[i].friendly;
                return enemy_monster;
            }
            r -= map_monster_data_sheet.list[i].weight;
        }
        return null;
    }


    // 1~4体の敵を生成し、画像をセット
    public void SetEnemyMonsters()
    {
        SetActiveCursors();
        if (enemy_monsters.Count == 0) {
            num_enemy_monsters = Random.Range(1,max_num_monsters+1);
        } else {
            num_enemy_monsters = enemy_monsters.Count;
        }
        List<List<Vector2>> anchors = new List<List<Vector2>>();

        // 敵の配置位置の定義
        switch (num_enemy_monsters) {
            case 1: 
                anchors.Add(new List<Vector2>(){new Vector2(0.5f-0.075f, 0.2f), new Vector2(0.5f+0.075f, 0.7f)});
                break;
            case 2:
                anchors.Add(new List<Vector2>(){new Vector2(0.5f-0.2f, 0.2f), new Vector2(0.5f-0.05f, 0.7f)});
                anchors.Add(new List<Vector2>(){new Vector2(0.5f+0.05f, 0.2f), new Vector2(0.5f+0.2f, 0.7f)});
                break;
            case 3:
                anchors.Add(new List<Vector2>(){new Vector2(0.5f-0.3f, 0.2f), new Vector2(0.5f-0.15f, 0.7f)});
                anchors.Add(new List<Vector2>(){new Vector2(0.5f-0.075f, 0.2f), new Vector2(0.5f+0.075f, 0.7f)});
                anchors.Add(new List<Vector2>(){new Vector2(0.5f+0.15f, 0.2f), new Vector2(0.5f+0.3f, 0.7f)});
                break;
            case 4:
                break;
        }
        if (enemy_monsters.Count == 0) {
            for (int i = 0; i < num_enemy_monsters; i++)
            {
                EnemyMonster enemy_monster = GenerateMonsterByWeight();
                enemy_monster.SetImage(enemy_objects[i].GetComponent<RawImage>());
                enemy_objects[i].SetActive(true);
                if (num_enemy_monsters < 4)
                {
                    enemy_objects[i].GetComponent<RectTransform>().anchorMin = anchors[i][0];
                    enemy_objects[i].GetComponent<RectTransform>().anchorMax = anchors[i][1];
                }
                enemy_monster.GetImage().texture = Resources.Load<Texture2D> (enemy_monster.image_path);
                enemy_monsters.Add(enemy_monster);
            }
        }
        else {
            for (int i = 0; i < num_enemy_monsters; i++) {
                enemy_monsters[i].SetImage(enemy_objects[i].GetComponent<RawImage>());
                enemy_objects[i].SetActive(true);
                if (num_enemy_monsters < 4)
                {
                    enemy_objects[i].GetComponent<RectTransform>().anchorMin = anchors[i][0];
                    enemy_objects[i].GetComponent<RectTransform>().anchorMax = anchors[i][1];
                }
                enemy_monsters[i].GetImage().texture = Resources.Load<Texture2D> (enemy_monsters[i].image_path);
            }
        }
    }

    public void FocusEnemy(Vector3 mousePos)
    {
        foreach(GameObject enemy_object in enemy_objects)
        {
            if (enemy_object.GetComponent<RawImage>().enabled == false) continue;
            Rect enemy_size = enemy_object.GetComponent<RectTransform>().rect;
            Vector3 relativeMousePos = enemy_object.transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= enemy_size.xMin && relativeMousePos.x <= enemy_size.xMax) &&
            (relativeMousePos.y >= enemy_size.yMin && relativeMousePos.y <= enemy_size.yMax)) 
            {
                BlinkMonsterCursor(enemy_object);
                return;
            }
        }
        ClearCursors();
    }

    public EnemyMonster ClickEnemy(Vector3 mousePos)
    {
        foreach(EnemyMonster enemy_monster in enemy_monsters)
        {
            if (enemy_monster.GetImage().enabled == false) continue;
            GameObject enemy_object =  enemy_monster.GetImage().gameObject;
            Rect enemy_size = enemy_object.GetComponent<RectTransform>().rect;
            Vector3 relativeMousePos = enemy_object.transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= enemy_size.xMin && relativeMousePos.x <= enemy_size.xMax) &&
            (relativeMousePos.y >= enemy_size.yMin && relativeMousePos.y <= enemy_size.yMax)) 
            {
                return enemy_monster;
            }
        }
        return null;
    }

    public void ClearCursors()
    {
        for (int i = 0; i < enemy_objects.Count; i++)
        {
            enemy_objects[i].transform.Find("Cursor").GetComponent<RawImage>().enabled = false;
        }
    }

    public void SetActiveCursors()
    {
        for (int i = 0; i < enemy_objects.Count; i++){
            enemy_objects[i].transform.Find("Cursor").gameObject.SetActive(true);
            enemy_objects[i].transform.Find("Cursor").GetComponent<RawImage>().enabled = false;
        }
    }

    public void BlinkMonsterCursor(GameObject enemy_object)
    {
        foreach (GameObject obj in enemy_objects)
        {
            if (obj != enemy_object) obj.transform.Find("Cursor").GetComponent<RawImage>().enabled = false;
        }
        RawImage cursor_image = enemy_object.transform.Find("Cursor").GetComponent<RawImage>();
        cursor_image.enabled = !cursor_image.enabled;
    }

    public IEnumerator BlinkMonster(Monster monster, float time)
    {
        // 点滅中なら抜ける
        if (monster.blinking) yield break;
        monster.blinking = true;
        RawImage monster_image = monster.GetImage();
        monster_image.enabled = false;
        yield return  new WaitForSeconds(time);
        monster_image.enabled = true;
        yield return  new WaitForSeconds(time);
        monster_image.enabled = false;
        yield return new WaitForSeconds(time);
        monster_image.enabled = true;
        monster.blinking = false;
    }

    public void DownPlayerMonsterWindow(PlayerMonster player_monster)
    {
        if (player_monster.isFocus) return;
        player_monster.isFocus = true;
        Vector3 pos = player_monster.GetStatusWindow().transform.position;
        pos.y -= 5;
        player_monster.GetStatusWindow().transform.position = pos;

    }

    public void UpPlayerMonsterWindow(PlayerMonster player_monster)
    {
        if (!player_monster.isFocus) return;
        player_monster.isFocus = false;
        Vector3 pos = player_monster.GetStatusWindow().transform.position;
        pos.y += 5;
        player_monster.GetStatusWindow().transform.position = pos;
    }

    public IEnumerator UpDownPlayerMonsterWindow(PlayerMonster player_monster, float time)
    {
        if (player_monster.isFocus) yield break;
        player_monster.isFocus = true;
        Vector3 pos = player_monster.GetStatusWindow().transform.position;
        pos.y -= 5;
        player_monster.GetStatusWindow().transform.position = pos;
        yield return new WaitForSeconds(time);
        pos.y += 5;
        player_monster.GetStatusWindow().transform.position = pos;
        player_monster.isFocus = false;
    }


    public void UpdateStatusWindow()
    {
        GameObject status_window;

        // 最大HPに対する現在のHPの割合
        float hp_ratio;
        foreach(PlayerMonster player_monster in player_monsters) {
            status_window = player_monster.GetStatusWindow();
            hp_ratio = (float)player_monster.hp / (float)player_monster.max_hp;
            status_window.transform.Find("HpText").GetComponent<TextMeshProUGUI>().text = "HP: " + player_monster.hp;
            status_window.transform.Find("MpText").GetComponent<TextMeshProUGUI>().text = "MP: " + player_monster.mp;
            if (hp_ratio > 0.5f) {
                continue;
            } 
            else if (hp_ratio > 0.2f) {
                status_window.GetComponent<Outline>().effectColor = Color.yellow;
                status_window.transform.Find("NameText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
                status_window.transform.Find("HpText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
                status_window.transform.Find("MpText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
            }
            else if (hp_ratio > 0) {
                status_window.GetComponent<Outline>().effectColor = orange;
                status_window.transform.Find("NameText").GetComponent<TextMeshProUGUI>().color = orange;
                status_window.transform.Find("HpText").GetComponent<TextMeshProUGUI>().color = orange;
                status_window.transform.Find("MpText").GetComponent<TextMeshProUGUI>().color = orange;
            }
            else {
                status_window.GetComponent<Outline>().effectColor = Color.red;
                status_window.transform.Find("NameText").GetComponent<TextMeshProUGUI>().color = Color.red;
                status_window.transform.Find("HpText").GetComponent<TextMeshProUGUI>().color = Color.red;
                status_window.transform.Find("MpText").GetComponent<TextMeshProUGUI>().color = Color.red;
            }
        }
        
    }

    public void ReviveMonsters() {
        foreach(PlayerMonster player_monster in player_monsters) {
            if (player_monster.isDead) {
                player_monster.isDead = false;
                player_monster.hp += 1;
            }
        }
    }

    // monster１体のレベルアップ処理
    public void HandleLevelUp(PlayerMonster player_monster, int obtain_exp)
    {
        player_monster.level_up = false;
        player_monster.now_exp += obtain_exp;
        player_monster.total_exp += obtain_exp;
        if (player_monster.now_exp > player_monster.need_exp) player_monster.level_up = true;
        while (player_monster.now_exp >= player_monster.need_exp) {
            player_monster.now_exp -= player_monster.need_exp;
            player_monster.level++;
            player_monster.need_exp = each_monster_data.sheets.Find(sheet=>sheet.name==player_monster.id.ToString()).list.Find(param=>param.lv==player_monster.level).need_exp;
        }
        player_monster.SetNewStatus(each_monster_data.sheets.Find(sheet=>sheet.name==player_monster.id.ToString()).list.Find(param=>param.lv==player_monster.level));
    }

    public bool HandleExpProcess()
    {
        int obtain_exp = 0;
        // どれかがレベルアップしたらtrue
        bool level_up = false;
        foreach (EnemyMonster enemy_monster in enemy_monsters) {
            obtain_exp += enemy_monster.exp;
        }
        foreach (PlayerMonster player_monster in player_monsters) {
            if (!player_monster.isDead && player_monster.level < 10) {
                HandleLevelUp(player_monster, obtain_exp);
                level_up &= player_monster.level_up;
            }
            // if(HandleLevelUp(player_monster, obtain_exp)) {
            //     Debug.Log(player_monster.name_ja + "のレベルがアップ" + player_monster.level);
            // }
        }
        return level_up;
    }

    public PlayerMonster ChangeToPlayerMonster(EnemyMonster enemy_monster)
    {
        EachMonsterData.Param u_param = each_monster_data.sheets.Find(sheet=>sheet.name==enemy_monster.id.ToString()).list.Find(param=>param.lv==enemy_monster.level);
        MonsterData.Param param = monster_data.sheets[0].list.Find(param=>param.id==enemy_monster.id);
        PlayerMonster player_monster = new PlayerMonster(u_param, param);
        return player_monster;
    }
}
