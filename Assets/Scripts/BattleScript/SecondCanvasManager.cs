using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SecondCanvasManager
{
    private class Item 
    {
        public TextMeshProUGUI item_text;
        public Skill skill;
        public Item(TextMeshProUGUI item_text, Skill skill) {
            this.skill = skill;
            this.item_text = item_text;
        }
    }

    private Canvas canvas;
    private GameObject window;
    // private List<TextMeshProUGUI> items;
    private List<Item> items;
    private GameObject toLeft;
    private GameObject toRight;
    private GameObject escape_cross;
    private int page;

    // コンストラクタ
    public SecondCanvasManager(Canvas canvas) 
    {
        this.canvas = canvas;
        canvas.gameObject.SetActive(false);
        window = canvas.transform.Find("SecondWindow").gameObject;
        items = new List<Item>();
        for (int i = 0; i < 8; i++) {
            items.Add(new Item(window.transform.Find($"Item{i+1}").gameObject.GetComponent<TextMeshProUGUI>(), null));
        }
        toLeft = window.transform.Find("Left").gameObject;
        toRight = window.transform.Find("Right").gameObject;
        escape_cross = window.transform.Find("EscapeCross").gameObject;

        // items[0].gameObject.SetActive(true);
    }

    public void OpenSkillWindow(Monster monster) 
    {
        page = 0;
        canvas.gameObject.SetActive(true);
        toLeft.GetComponent<RawImage>().enabled = false;
        if (monster.skills.Count < 8) {
            for (int i = 0; i < monster.skills.Count; i++) {
                items[i].item_text.text = monster.skills[i].param.name_ja;
                items[i].item_text.enabled = true;
                items[i].skill = monster.skills[i];
            }
            for (int i = monster.skills.Count; i < 8; i++) {
                items[i].item_text.enabled = false;
            }
            toRight.GetComponent<RawImage>().enabled = false;
        } else {
            for (int i = 0; i < 8; i++) {
                items[i].item_text.text = monster.skills[i].param.name_ja;
                items[i].item_text.enabled = true;
                items[i].skill = monster.skills[i];
            }
            toRight.GetComponent<RawImage>().enabled = true;
        }
    }

    public void BackSkillWindow(Vector3 mousePos, Monster monster)
    {
        if (toLeft.activeSelf == false || toLeft.activeInHierarchy == false) return;
        Vector3 relativeMousePos = toLeft.transform.InverseTransformPoint(mousePos);
        Vector2 to_left_size = toLeft.GetComponent<RectTransform>().sizeDelta;

        // toLeftが押された場合
        if ((relativeMousePos.x >= -(to_left_size.x / 2) && relativeMousePos.x <= to_left_size.x / 2) &&
        (relativeMousePos.y >= -(to_left_size.y / 2) && relativeMousePos.y <= to_left_size.y / 2)
        ) {
            toRight.GetComponent<RawImage>().enabled = true;
            page--;

            // 前のページが存在しない場合
            if (page == 0) {
                for (int i = 0; i < 8; i++) {
                    items[i].item_text.text = monster.skills[i + 8 * page].param.name_ja;
                    items[i].skill = monster.skills[i + 8 * page];
                    items[i].item_text.enabled = true;
                }
                toLeft.GetComponent<RawImage>().enabled = false;
            } else if (page > 0) {
                for (int i = 0; i < 8; i++) {
                    items[i].item_text.text = monster.skills[i + 8 * page].param.name_ja;
                    items[i].skill = monster.skills[i + 8 * page];
                    items[i].item_text.enabled = true;
                }
                toLeft.GetComponent<RawImage>().enabled = true;
            } else {
                Debug.LogError("Error: the page does not exist");
            }
        }
    }

    public void NextSkillWindow(Vector3 mousePos, Monster monster)
    {
        if (toRight.activeSelf == false || toRight.activeInHierarchy == false) return;
        Vector3 relativeMousePos = toRight.transform.InverseTransformPoint(mousePos);
        Vector2 to_right_size = toRight.GetComponent<RectTransform>().sizeDelta;

        // toRightが押された場合
        if ((relativeMousePos.x >= -(to_right_size.x / 2) && relativeMousePos.x <= to_right_size.x / 2) &&
        (relativeMousePos.y >= -(to_right_size.y / 2) && relativeMousePos.y <= to_right_size.y / 2)
        ) {
            toLeft.GetComponent<RawImage>().enabled = true;
            page++;

            // 次のページが必要ない場合
            if (monster.skills.Count < 8 * (page + 1)) {
                for (int i = 0; i < monster.skills.Count - 8 * page; i++) {
                    items[i].item_text.text = monster.skills[i + 8 * page].param.name_ja;
                    items[i].skill = monster.skills[i + 8 * page];
                    items[i].item_text.enabled = true;
                }
                for (int i = monster.skills.Count - 8 * page; i < 8; i++) {
                    items[i].item_text.enabled = false;
                }
                toRight.GetComponent<RawImage>().enabled = false;
            } 
            // 次のページが必要な場合
            else {
                for (int i = 0; i < 8; i++) {
                    items[i].item_text.text = monster.skills[i + 8 * page].param.name_ja;
                    items[i].item_text.enabled = true;
                    items[i].skill = monster.skills[i + 8 * page];
                }
            }
        }
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

    public Skill SelectSkill(Vector3 mousePos)
    {
        // 選ぶ選択肢がない場合
        if (items[0].item_text.enabled == false) {
            return null;
        }
        Vector2 item_block_size = items[0].item_text.gameObject.GetComponent<RectTransform>().sizeDelta;
        Vector3 relativeMousePos;
        for (int i = 0; i < items.Count; i++) {
            if (items[i].item_text.enabled == false) {
                continue;
            }
            relativeMousePos = items[i].item_text.transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= -(item_block_size.x / 2) && relativeMousePos.x <= item_block_size.x / 2) &&
            (relativeMousePos.y >= -(item_block_size.y / 2) && relativeMousePos.y <= item_block_size.y / 2)
            ) {
                canvas.gameObject.SetActive(false);
                return items[i].skill;
            }
        }
        return null;
    }
}
