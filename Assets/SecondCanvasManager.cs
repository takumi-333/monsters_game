using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SecondCanvasManager
{
    private Canvas canvas;
    private GameObject window;
    private List<TextMeshProUGUI> items;
    private GameObject toLeft;
    private GameObject toRight;
    private GameObject escape_cross;
    private int page;


    // コンストラクタ
    public SecondCanvasManager(Canvas canvas) 
    {
        this.canvas = canvas;
        window = canvas.transform.Find("SecondWindow").gameObject;
        items = new List<TextMeshProUGUI>();
        for (int i = 0; i < 8; i++) {
            items.Add(window.transform.Find($"Item{i+1}").gameObject.GetComponent<TextMeshProUGUI>());
        }
        toLeft = window.transform.Find("Left").gameObject;
        toRight = window.transform.Find("Right").gameObject;
        escape_cross = window.transform.Find("EscapeCross").gameObject;

        items[0].gameObject.SetActive(true);
    }

    public void OpenSkillWindow(Monster monster) 
    {
        page = 0;
        canvas.gameObject.SetActive(true);
        toLeft.GetComponent<RawImage>().enabled = false;
        if (monster.skills.Count < 8) {
            for (int i = 0; i < monster.skills.Count; i++) {
                items[i].text = monster.skills[i].param.name_ja;
                items[i].enabled = true;
            }
            for (int i = monster.skills.Count; i < 8; i++) {
                items[i].enabled = false;
            }
            toRight.GetComponent<RawImage>().enabled = false;
        } else {
            for (int i = 0; i < 8; i++) {
                items[i].text = monster.skills[i].param.name_ja;
            }
        }
    }

    public void BackSkillWindow(Monster monster, int page)
    {

    }

    public void NextSkillWindow(Monster monster, int page)
    {

    }

    public bool CloseWindow(Vector3 mousePos) 
    {
        Vector3 relativeMousePos = escape_cross.transform.InverseTransformPoint(mousePos);
        Vector2 escape_cross_size = escape_cross.GetComponent<RectTransform>().sizeDelta;
        if ((relativeMousePos.x >= -(escape_cross_size.x / 2) && relativeMousePos.x <= escape_cross_size.x / 2) &&
        (relativeMousePos.y >= -(escape_cross_size.y / 2) && relativeMousePos.y <= escape_cross_size.y / 2)
        ) {
            canvas.gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    public int SelectSkills(Vector3 mousePos)
    {
        // 選ぶ選択肢がない場合
        if (items[0].enabled == false) {
            return -1;
        }
        Vector2 item_block_size = items[0].gameObject.GetComponent<RectTransform>().sizeDelta;
        // Vector3 relativeMousePos = escape_cross.transform.InverseTransformPoint(mousePos);
        Vector3 relativeMousePos;
        for (int i = 0; i < items.Count; i++) {
            if (items[i].enabled == false) {
                continue;
            }
            relativeMousePos = items[i].transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= -(item_block_size.x / 2) && relativeMousePos.x <= item_block_size.x / 2) &&
            (relativeMousePos.y >= -(item_block_size.y / 2) && relativeMousePos.y <= item_block_size.y / 2)
            ) {
                canvas.gameObject.SetActive(false);
                return 8*page + i;
            }
        }
        return -1;
        
    }
}
