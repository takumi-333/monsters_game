using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FarmCanvasManager
{
    private class MonsterObject {
        public PlayerMonster monster;
        public GameObject monster_window;
        public bool inFarm;
        public int index;

        public MonsterObject(GameObject monster_window) {
            this.monster_window =  monster_window;
            monster = null;
            inFarm = true;
        }
    }

    private Canvas canvas;
    private MonsterData monster_data;
    private EachMonsterData each_monster_data;

    private GameObject farm_window;
    private GameObject close_button;
    private GameObject next_button;
    private GameObject prev_button;
    private List<MonsterObject> farm_monster_objects;
    private FarmData farm_data;
    private MonsterObject holded_object;
    private Vector3 original_pos;
    private int page;

    private GameObject player_window;
    private List<PlayerMonster> player_monsters;
    private List<MonsterObject> player_monster_objects;

    public FarmCanvasManager(Canvas canvas, List<PlayerMonster> player_monsters, FarmData farm_data) {
        this.canvas = canvas;
        monster_data = Resources.Load("monster_data") as MonsterData;
        each_monster_data = Resources.Load("each_monster_data") as EachMonsterData;
    
        farm_window = canvas.transform.Find("FarmWindow").gameObject;
        player_window = canvas.transform.Find("PlayerWindow").gameObject;

        close_button = farm_window.transform.Find("CloseButton").gameObject;
        prev_button = farm_window.transform.Find("PrevButton").gameObject;
        next_button = farm_window.transform.Find("NextButton").gameObject;
        farm_monster_objects = new List<MonsterObject>();
        for (int i = 0; i < 12; i++) {
            farm_monster_objects.Add(new MonsterObject(farm_window.transform.Find($"FarmMonster{i}").gameObject));
        }

        player_monster_objects = new List<MonsterObject>();
        for (int i = 0; i < 4; i++) {
            player_monster_objects.Add(new MonsterObject(player_window.transform.Find($"Monster{i}").gameObject));
        }
        holded_object = null;

        this.player_monsters = player_monsters;
        this.farm_data = farm_data;
    }

    public void OpenFarmCanvas()
    {
        canvas.enabled = true;
        page = 0;
        SetPlayerMonster();
        SetFarmMonster();
        prev_button.SetActive(false);
        if (farm_data.num_farm_monsters <= 12) {
            next_button.SetActive(false);
        }
    }

    public void SetEmptyPlayerMonster(int index) 
    {
        GameObject player_monster_window = player_monster_objects[index].monster_window;
        RawImage monster_image = player_monster_window.transform.Find("MonsterImage").GetComponent<RawImage>();
        TextMeshProUGUI monster_name = player_monster_window.transform.Find("MonsterName").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI monster_level = player_monster_window.transform.Find("MonsterLevel").GetComponent<TextMeshProUGUI>();
        monster_image.enabled = false;
        monster_name.enabled = false;
        monster_level.enabled = false;
    }

    public void SetPlayerMonster() 
    {
        GameObject player_monster_window;
        for (int i = 0; i < player_monsters.Count; i++) {
            player_monster_window = player_monster_objects[i].monster_window;
            player_monster_objects[i].monster = player_monsters[i];
            player_monster_objects[i].inFarm = false;
            player_monster_objects[i].index = i;
            RawImage monster_image = player_monster_window.transform.Find("MonsterImage").GetComponent<RawImage>();
            TextMeshProUGUI monster_name = player_monster_window.transform.Find("MonsterName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI monster_level = player_monster_window.transform.Find("MonsterLevel").GetComponent<TextMeshProUGUI>();
            monster_image.enabled = true;
            monster_name.enabled = true;
            monster_level.enabled = true;
            monster_image.texture = Resources.Load<Texture2D>(player_monsters[i].image_path);
            monster_name.text = player_monsters[i].name_ja;
            monster_level.text = "Lv." + player_monsters[i].level;
        }
        for (int i = player_monsters.Count; i< 4; i++) {
            player_monster_objects[i].inFarm = false;
            SetEmptyPlayerMonster(i);
        }
    }

    public void SetFarmMonster()
    {
        Debug.Log("SetFarmMonster");
        GameObject farm_monster_window;
        for (int i = 0; i < 12; i++) {
            farm_monster_window = farm_monster_objects[i].monster_window;
            farm_monster_objects[i].index = 12*page + i;
            // セットするモンスターがいる箇所
            if (12*page+i < farm_data.num_farm_monsters) {
                farm_monster_window.SetActive(true);
                RawImage monster_image = farm_monster_window.transform.Find("MonsterImage").GetComponent<RawImage>();
                TextMeshProUGUI monster_name = farm_monster_window.transform.Find("MonsterName").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI monster_level = farm_monster_window.transform.Find("MonsterLevel").GetComponent<TextMeshProUGUI>();

                FarmData.FarmMonsterData farm_monster_data = farm_data.farm_monster_datas[12*page + i];
                MonsterData.Param param = monster_data.sheets[0].list.Find(param=>param.id==farm_monster_data.id);
                EachMonsterData.Param u_param = each_monster_data.sheets.Find(sheet=>sheet.name==farm_monster_data.id.ToString()).list.Find(param=>param.lv==farm_monster_data.level);
                PlayerMonster pmon = new PlayerMonster(u_param, param);
                pmon.uuid = farm_monster_data.uuid;
                pmon.total_exp = farm_monster_data.total_exp;
                pmon.need_exp = farm_monster_data.need_exp;
                pmon.now_exp = farm_monster_data.now_exp;
                pmon.name_ja = farm_monster_data.name_ja;
                farm_monster_objects[i].monster = pmon;

                monster_image.texture = Resources.Load<Texture2D>(farm_data.farm_monster_datas[12*page+i].image_path);
                monster_name.text = farm_data.farm_monster_datas[12*page + i].name_ja;
                monster_level.text = "Lv." + farm_data.farm_monster_datas[12*page + i].level;
            } 
            // セットするモンスターがいない箇所
            else {
                farm_monster_window.SetActive(false);
            }
        }
        Debug.Log("Ended SetFarmMonster");
    }

    public bool ClickCloseButton(Vector3 mousePos) 
    {
        if (close_button.activeInHierarchy == false) return false;
        Rect button_size = close_button.GetComponent<RectTransform>().rect;
        Vector3 relativeMousePos = close_button.transform.InverseTransformPoint(mousePos);
        if ((relativeMousePos.x >= button_size.xMin && relativeMousePos.x <= button_size.xMax) &&
            (relativeMousePos.y >= button_size.yMin && relativeMousePos.y <= button_size.yMax)) 
        {
            canvas.enabled = false;
            return true;
        }
        return false;
    }

    public void HoldWindow(Vector3 mousePos)
    {
        Rect window_size;
        Vector3 relativeMousePos;
        foreach (MonsterObject player_monster_object in player_monster_objects)
        {
            GameObject player_monster_window = player_monster_object.monster_window;
            if (player_monster_window.transform.Find("MonsterImage").GetComponent<RawImage>().enabled == false) {
                continue;
            }
            window_size = player_monster_window.GetComponent<RectTransform>().rect;
            relativeMousePos = player_monster_window.transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= window_size.xMin && relativeMousePos.x <= window_size.xMax) &&
            (relativeMousePos.y >= window_size.yMin && relativeMousePos.y <= window_size.yMax)) 
            {
                holded_object = player_monster_object;
                holded_object.monster_window.transform.SetAsLastSibling();
                player_window.transform.SetAsLastSibling();
                original_pos = player_monster_window.transform.position;
            }
        }
        foreach (MonsterObject farm_monster_object in farm_monster_objects)
        {
            GameObject farm_monster_window = farm_monster_object.monster_window;
            if (farm_monster_window.activeSelf == false) continue;
            window_size = farm_monster_window.GetComponent<RectTransform>().rect;
            relativeMousePos = farm_monster_window.transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= window_size.xMin && relativeMousePos.x <= window_size.xMax) &&
            (relativeMousePos.y >= window_size.yMin && relativeMousePos.y <= window_size.yMax)) 
            {
                holded_object = farm_monster_object;
                holded_object.monster_window.transform.SetAsLastSibling();
                farm_window.transform.SetAsLastSibling();
                original_pos = farm_monster_window.transform.position;
            }
        }
    }

    public void UpdateMonsters()
    {
        Debug.Log("Update monsters");
        Debug.Log(player_monsters.Count);
        for (int i = 0; i < 4; i++) {
            GameObject player_monster_window = player_monster_objects[i].monster_window;
            player_monster_objects[i].inFarm = false;
            if (i < player_monsters.Count)  player_monster_objects[i].monster = player_monsters[i];
            else player_monster_objects[i].monster = null;
            RawImage monster_image = player_monster_window.transform.Find("MonsterImage").GetComponent<RawImage>();
            TextMeshProUGUI monster_name = player_monster_window.transform.Find("MonsterName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI monster_level = player_monster_window.transform.Find("MonsterLevel").GetComponent<TextMeshProUGUI>();
            if (player_monster_objects[i].monster != null) {
                Debug.Log("set monster");
                monster_image.enabled = true;
                monster_name.enabled = true;
                monster_level.enabled = true;
                monster_image.texture = Resources.Load<Texture2D>(player_monsters[i].image_path);
                monster_name.text = player_monsters[i].name_ja;
                monster_level.text = "Lv." + player_monsters[i].level;
            } else {
                Debug.Log("none monster");
                monster_image.enabled = false;
                monster_name.enabled = false;
                monster_level.enabled = false;
            }
        }
        SetFarmMonster();
        Debug.Log("Finished Update monsters");
    }

    public void MoveWindow(Vector3 mousePos)
    {
        if (holded_object != null) holded_object.monster_window.transform.position = mousePos;
    }

    // holdから離した時入れ替えが起こる
    public void ReleaseWindow(Vector3 mousePos)
    {

        // 入れ替えのチェック
        if (ChangePosition(mousePos)) {
            UpdateMonsters();
        }
        if (holded_object != null) {
            holded_object.monster_window.transform.position = original_pos;
            holded_object = null;
        }
    }

    // 入れ替えがあればtrue、入れ替えがなければfalseを返す
    public bool ChangePosition(Vector3 mousePos) {
        Rect window_size;
        Vector3 relativeMousePos;
        // 交換元がfarmのモンスターのとき
        if (holded_object == null) return false;
        if (holded_object.inFarm) 
        {
            for (int i = 0; i < 4; i++)
            {
                if (player_monster_objects[i] == holded_object) continue;
                GameObject player_monster_window = player_monster_objects[i].monster_window;
                
                window_size = player_monster_window.GetComponent<RectTransform>().rect;
                relativeMousePos = player_monster_window.transform.InverseTransformPoint(mousePos);
                // 移動があった場合( player_monsterの方へ)
                if ((relativeMousePos.x >= window_size.xMin && relativeMousePos.x <= window_size.xMax) &&
                (relativeMousePos.y >= window_size.yMin && relativeMousePos.y <= window_size.yMax)) 
                {
                    // 交換先がemptyのとき
                    if (player_monster_window.transform.Find("MonsterImage").GetComponent<RawImage>().enabled == false) {
                        player_monsters.Add(holded_object.monster);
                        holded_object.monster = null;
                        Debug.Log(holded_object.index);
                        farm_data.RemoveMonsterDataAt(holded_object.index);
                        return true;
                    } 
                    // 交換先がいるとき
                    else {
                        PlayerMonster tmp_monster;
                        tmp_monster = holded_object.monster;
                        holded_object.monster = player_monster_objects[i].monster;
                        player_monster_objects[i].monster = tmp_monster;
                        player_monsters[i] = player_monster_objects[i].monster;
                        farm_data.ChangeMonsterDataAt(holded_object.monster, holded_object.index);
                        return true;
                    }
                }
            }
            foreach (MonsterObject farm_monster_object in farm_monster_objects)
            {
                if (farm_monster_object == holded_object) continue;
                GameObject farm_monster_window = farm_monster_object.monster_window;
                if (farm_monster_window.activeSelf == false) continue;
                window_size = farm_monster_window.GetComponent<RectTransform>().rect;
                relativeMousePos = farm_monster_window.transform.InverseTransformPoint(mousePos);
                // 移動があった場合( farm_monsterの方へ)
                if ((relativeMousePos.x >= window_size.xMin && relativeMousePos.x <= window_size.xMax) &&
                (relativeMousePos.y >= window_size.yMin && relativeMousePos.y <= window_size.yMax)) 
                {
                    // farm_monster内での入れ替え
                    PlayerMonster tmp_monster;
                    tmp_monster = holded_object.monster;
                    farm_data.ChangeMonsterDataAt(tmp_monster, farm_monster_object.index);
                    farm_data.ChangeMonsterDataAt(farm_monster_object.monster, holded_object.index);
                    return true;
                }
            }
        }
        // 交換元がplayerのモンスターのとき
        else {
            for (int i = 0; i < 4; i++)
            {
                if (player_monster_objects[i] == holded_object) continue;
                GameObject player_monster_window = player_monster_objects[i].monster_window;
                // player_monsterの空白とは入れ替えられない
                window_size = player_monster_window.GetComponent<RectTransform>().rect;
                relativeMousePos = player_monster_window.transform.InverseTransformPoint(mousePos);
                // 移動があった場合( player_monsterの方へ)
                if ((relativeMousePos.x >= window_size.xMin && relativeMousePos.x <= window_size.xMax) &&
                (relativeMousePos.y >= window_size.yMin && relativeMousePos.y <= window_size.yMax)) 
                {
                    // 交換先がemptyのとき
                    if (player_monster_objects[i].monster == null) {
                        player_monsters.Remove(holded_object.monster);
                        player_monsters.Add(holded_object.monster);
                        return true;
                    } 
                    //交換先がいるとき
                    else {
                        PlayerMonster tmp_monster;
                        tmp_monster = holded_object.monster;
                        player_monsters[holded_object.index] = player_monsters[i];
                        player_monsters[i] = tmp_monster;
                        return true;
                    }
                    
                }
            }
            foreach (MonsterObject farm_monster_object in farm_monster_objects)
            {
                if (farm_monster_object == holded_object) continue;
                GameObject farm_monster_window = farm_monster_object.monster_window;
                if (farm_monster_window.activeSelf == false) continue;
                window_size = farm_monster_window.GetComponent<RectTransform>().rect;
                relativeMousePos = farm_monster_window.transform.InverseTransformPoint(mousePos);
                // 移動があった場合( farm_monsterの方へ)
                if ((relativeMousePos.x >= window_size.xMin && relativeMousePos.x <= window_size.xMax) &&
                (relativeMousePos.y >= window_size.yMin && relativeMousePos.y <= window_size.yMax)) 
                {
                    // farm_monsterとの入れ替え
                    PlayerMonster tmp_monster;
                    tmp_monster = holded_object.monster;
                    player_monsters[holded_object.index] = farm_monster_object.monster;
                    farm_data.ChangeMonsterDataAt(tmp_monster, farm_monster_object.index);
                    return true;
                }
            }
            // farm_windowの空白部分にドロップした場合
            window_size = farm_window.GetComponent<RectTransform>().rect;
            relativeMousePos = farm_window.transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= window_size.xMin && relativeMousePos.x <= window_size.xMax) &&
            (relativeMousePos.y >= window_size.yMin && relativeMousePos.y <= window_size.yMax))
            {
                // player_monsterが2体以上いてfarmに空きがある場合
                if (player_monsters.Count > 1 && farm_data.num_farm_monsters < 60) {
                    player_monsters.Remove(holded_object.monster);
                    farm_data.AddMonsterData(holded_object.monster);
                    return true;
                } else {
                    return false;
                }
            }
                

        }
        return false;
    }

    public bool ClickPrevButton(Vector3 mousePos)
    {
        if (close_button.activeInHierarchy == false || prev_button.activeSelf == false) return false;
        Rect button_size = prev_button.GetComponent<RectTransform>().rect;
        Vector3 relativeMousePos = prev_button.transform.InverseTransformPoint(mousePos);
        if ((relativeMousePos.x >= button_size.xMin && relativeMousePos.x <= button_size.xMax) &&
            (relativeMousePos.y >= button_size.yMin && relativeMousePos.y <= button_size.yMax)) 
        {
            page--;
            if (page == 0) {
                prev_button.SetActive(false);
            }
            SetFarmMonster();
            next_button.SetActive(true);
            return true;
        }
        return false;
    }

    public bool ClickNextButton(Vector3 mousePos)
    {
        if (next_button.activeInHierarchy == false || next_button.activeSelf == false) return false;
        Rect button_size = next_button.GetComponent<RectTransform>().rect;
        Vector3 relativeMousePos = next_button.transform.InverseTransformPoint(mousePos);
        if ((relativeMousePos.x >= button_size.xMin && relativeMousePos.x <= button_size.xMax) &&
            (relativeMousePos.y >= button_size.yMin && relativeMousePos.y <= button_size.yMax)) 
        {
            page++;
            if (farm_data.num_farm_monsters <= (page + 1) * 12) {
                next_button.SetActive(false);
            }
            prev_button.SetActive(true);
            SetFarmMonster();
            return true;
        }
        return false;
    }

}
