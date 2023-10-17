using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MonsterManager
{
    public List<PlayerMonster> player_monsters;
    public List<EnemyMonster> enemy_monsters;
    // public List<PlayerMonster> dead_player_monsters;
    // public List<EnemyMonster> dead_enemy_monsters;

    private MonsterData monster_data;
    private SkillData skill_data;

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

    private Color orange = new Color(238f/255f, 120f/255f, 0);



    public MonsterManager(Canvas status_window_canvas, Canvas enemy_canvas)
    {
        this.status_window_canvas = status_window_canvas;
        this.enemy_canvas = enemy_canvas;

        player_monsters = new List<PlayerMonster>();
        enemy_monsters = new List<EnemyMonster>();
        // dead_player_monsters = new List<PlayerMonster>();
        // dead_enemy_monsters = new List<EnemyMonster>();
        num_dead_player_monsters = 0;
        num_dead_enemy_monsters = 0;

        monster_data = Resources.Load("monster_data") as MonsterData;
        skill_data = Resources.Load("skill_data") as SkillData;

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
            status_windows[i].transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = player_monsters[i].param.name_ja;
            status_windows[i].transform.Find("MonsterImage").GetComponent<RawImage>().texture = Resources.Load<Texture2D>(player_monsters[i].param.image_path);
            UpdateStatusWindow();
        }
    }

    // 出現するモンスターを出現確率から決定
    public EnemyMonster GenerateMonsterByWeight() 
    {
        int total_weight = 0;
        MonsterData.Param monster_param;
        for (int i = 0; i < monster_data.sheets[0].list.Count; i++) {
            total_weight += monster_data.sheets[0].list[i].weight;
        }
        int r = Random.Range(1, total_weight);
        for (int i = 0; i < monster_data.sheets[0].list.Count; i++) {
            if (r < monster_data.sheets[0].list[i].weight) {
                monster_param = monster_data.sheets[0].list[i];
                return new EnemyMonster(monster_param);
            }
            r -= monster_data.sheets[0].list[i].weight;
        }
        return null;
    }

    // 1~4体の敵を生成し、画像をセット
    public void SetEnemyMonsters()
    {
        SetActiveCursors();
        num_enemy_monsters = Random.Range(1,5);
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
            enemy_monster.GetImage().texture = Resources.Load<Texture2D> (enemy_monster.param.image_path);
            enemy_monsters.Add(enemy_monster);
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
            hp_ratio = (float)player_monster.param.hp / (float)player_monster.max_hp;
            status_window.transform.Find("HpText").GetComponent<TextMeshProUGUI>().text = "HP: " + player_monster.param.hp;
            status_window.transform.Find("MpText").GetComponent<TextMeshProUGUI>().text = "MP: " + player_monster.param.mp;
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
                player_monster.param.hp += 1;
            }
        }
    }
}
