using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusCanvasManager
{
    private Canvas canvas;
    private GameObject status_window;
    private GameObject close_button;
    private List<GameObject> monster_windows;
    private MonsterData monster_data;

    public StatusCanvasManager(Canvas canvas, List<PlayerMonster> player_monsters)
    {
        // モンスターの情報を獲得
        monster_data = Resources.Load("monster_data") as MonsterData;
        this.canvas = canvas;
        status_window = canvas.transform.Find("StatusWindow").gameObject;
        close_button = status_window.transform.Find("Close").gameObject;
        monster_windows = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            monster_windows.Add(status_window.transform.Find($"MonsterWindow{i}").gameObject);
            if (i >= player_monsters.Count) monster_windows[i].SetActive(false);
        }

        // player monstersの情報をセット
        SetPlayerMonstersStatus(player_monsters);
        canvas.gameObject.SetActive(false);
    }

    private void SetPlayerMonstersStatus(List<PlayerMonster> player_monsters)
    {
        RawImage monster_image;
        TextMeshProUGUI name_text;
        TextMeshProUGUI hp_text;
        TextMeshProUGUI mp_text;
        TextMeshProUGUI exp_text;
        for (int i = 0; i < player_monsters.Count; i++) {
            monster_image = monster_windows[i].transform.Find("MonsterImage").GetComponent<RawImage>();
            name_text = monster_windows[i].transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            hp_text = monster_windows[i].transform.Find("HpText").GetComponent<TextMeshProUGUI>();
            mp_text = monster_windows[i].transform.Find("MpText").GetComponent<TextMeshProUGUI>();
            exp_text = monster_windows[i].transform.Find("ExpText").GetComponent<TextMeshProUGUI>();

            monster_image.texture = Resources.Load<Texture2D>(player_monsters[i].param.image_path);
            name_text.text = player_monsters[i].param.name_ja;
            hp_text.text = "HP: " + player_monsters[i].param.hp + "/" + player_monsters[i].max_hp;
            mp_text.text = "MP: " + player_monsters[i].param.mp + "/" + player_monsters[i].max_mp;
        }
    }

    public void OpenStatusWindow()
    {
        canvas.gameObject.SetActive(true);
    }

    public void CloseStatusWindow(Vector3 mousePos) 
    {
        if (close_button.activeInHierarchy == false) return;
        Rect button_size = close_button.GetComponent<RectTransform>().rect;
        Vector3 relativeMousePos = close_button.transform.InverseTransformPoint(mousePos);
        if ((relativeMousePos.x >= button_size.xMin && relativeMousePos.x <= button_size.xMax) &&
            (relativeMousePos.y >= button_size.yMin && relativeMousePos.y <= button_size.yMax)) 
        {
            canvas.gameObject.SetActive(false);
        }
    }
}
