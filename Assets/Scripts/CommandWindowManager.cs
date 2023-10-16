using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandWindowManager
{
    private Canvas canvas;
    private GameObject command_window;
    private TextMeshProUGUI battle_message1;
    private TextMeshProUGUI battle_message2;
    private List<GameObject> commands;
    public bool displaying;
    private string[] command_str ;

    public enum CommandType 
    {
        None = -1,
        ATTACK = 0,
        ITEM = 1,
        SPECIAL = 2,
        ESCAPE = 3
    }

    public CommandWindowManager(Canvas canvas)
    {
        // 初期化
        this.canvas = canvas;
        command_window = canvas.transform.Find("CommandWindow").gameObject;
        battle_message1 = command_window.transform.Find("BattleMessage1").gameObject.GetComponent<TextMeshProUGUI>();
        battle_message2 = command_window.transform.Find("BattleMessage2").gameObject.GetComponent<TextMeshProUGUI>();

        battle_message1.enabled = false;
        battle_message2.enabled = false;
        displaying = false;
        command_str = new string[]{"こうげき", "どうぐ", "とくぎ", "にげる"};
        commands = new List<GameObject>();
        for (int i = 0; i < 4; i++) {
            GameObject command = command_window.transform.Find($"Command{i}").gameObject;
            command.transform.Find("CommandText").GetComponent<TextMeshProUGUI>().text = command_str[i];
            command.SetActive(false);
            commands.Add(command);
        }
    }
    
    public void SetBattleMessage1(string str)
    {
        battle_message1.text = str;
        battle_message1.enabled = true;
    }

    public void SetBattleMessage2(string str)
    {
        battle_message2.text = str;
        battle_message2.enabled = true;
    }

    public void ClearAllMessage()
    {
        battle_message1.text = "";
        battle_message2.text = "";
        battle_message1.enabled = false;
        battle_message2.enabled = false;
    }

    public IEnumerator DisplayMessage1(string message) 
    {
        if (displaying) yield break;
        displaying = true;
        battle_message1.text = "";
        battle_message1.enabled = true;
        battle_message1.text += message[0];
        for (int i = 1; i < message.Length; i++) {
            battle_message1.text += message[i];
            yield return new WaitForSeconds (0.1f);
        }
        yield return new WaitForSeconds (1.0f);
        battle_message1.enabled = false;
        displaying = false;
    }

    public IEnumerator DisplayMessage2(string message) 
    {
        battle_message2.text = "";
        battle_message2.enabled = true;
        battle_message2.text += message[0];
        for (int i = 1; i < message.Length; i++) {
            battle_message2.text += message[i];
            yield return new WaitForSeconds (0.1f);
        }
        yield return new WaitForSeconds (1.0f);
        battle_message2.enabled = false;
        displaying = false;
    }

    public void SetCommandsActive(bool b)
    {
        for (int i = 0; i < 4; i++)
        {
            commands[i].SetActive(b);
        }
    }

    public void SelectedCommandActive(int index)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == index) commands[i].SetActive(true);
            else commands[i].SetActive(false);
        }
    }

    public int HandleClickCommand(Vector3 mousePos) {
        int index = -1;

        foreach(GameObject command in commands) {
            index += 1;
            if (!command.activeSelf) continue;
            Rect command_block_size = command.GetComponent<RectTransform>().rect;
            Vector3 relativeMousePos = command.transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= command_block_size.xMin && relativeMousePos.x <= command_block_size.xMax) &&
            (relativeMousePos.y >= command_block_size.yMin && relativeMousePos.y <= command_block_size.yMax)) 
            {
                return index;
            }
        }
        return -1;
    }
   



    // public void OpenSkillWindow(Monster monster) 
    // {
    //     page = 0;
    //     canvas.gameObject.SetActive(true);
    //     toLeft.GetComponent<RawImage>().enabled = false;
    //     if (monster.skills.Count < 8) {
    //         for (int i = 0; i < monster.skills.Count; i++) {
    //             items[i].text = monster.skills[i].param.name_ja;
    //             items[i].enabled = true;
    //         }
    //         for (int i = monster.skills.Count; i < 8; i++) {
    //             items[i].enabled = false;
    //         }
    //         toRight.GetComponent<RawImage>().enabled = false;
    //     } else {
    //         for (int i = 0; i < 8; i++) {
    //             items[i].text = monster.skills[i].param.name_ja;
    //             items[i].enabled = true;
    //         }
    //         toRight.GetComponent<RawImage>().enabled = true;
    //     }
    // }

    // public void BackSkillWindow(Vector3 mousePos, Monster monster)
    // {
    //     if (toLeft.activeSelf == false || toLeft.activeInHierarchy == false) return;
    //     Vector3 relativeMousePos = toLeft.transform.InverseTransformPoint(mousePos);
    //     Vector2 to_left_size = toLeft.GetComponent<RectTransform>().sizeDelta;

    //     // toLeftが押された場合
    //     if ((relativeMousePos.x >= -(to_left_size.x / 2) && relativeMousePos.x <= to_left_size.x / 2) &&
    //     (relativeMousePos.y >= -(to_left_size.y / 2) && relativeMousePos.y <= to_left_size.y / 2)
    //     ) {
    //         toRight.GetComponent<RawImage>().enabled = true;
    //         page--;

    //         // 前のページが存在しない場合
    //         if (page == 0) {
    //             for (int i = 0; i < 8; i++) {
    //                 items[i].text = monster.skills[i + 8 * page].param.name_ja;
    //                 items[i].enabled = true;
    //             }
    //             toLeft.GetComponent<RawImage>().enabled = false;
    //         } else if (page > 0) {
    //             for (int i = 0; i < 8; i++) {
    //                 items[i].text = monster.skills[i + 8 * page].param.name_ja;
    //                 items[i].enabled = true;
    //             }
    //             toLeft.GetComponent<RawImage>().enabled = true;
    //         } else {
    //             Debug.Log("Error: the page does not exist");
    //         }
    //     }
    // }

    // public void NextSkillWindow(Vector3 mousePos, Monster monster)
    // {
    //     if (toRight.activeSelf == false || toRight.activeInHierarchy == false) return;
    //     Vector3 relativeMousePos = toRight.transform.InverseTransformPoint(mousePos);
    //     Vector2 to_right_size = toRight.GetComponent<RectTransform>().sizeDelta;

    //     // toRightが押された場合
    //     if ((relativeMousePos.x >= -(to_right_size.x / 2) && relativeMousePos.x <= to_right_size.x / 2) &&
    //     (relativeMousePos.y >= -(to_right_size.y / 2) && relativeMousePos.y <= to_right_size.y / 2)
    //     ) {
    //         toLeft.GetComponent<RawImage>().enabled = true;
    //         page++;

    //         // 次のページが必要ない場合
    //         if (monster.skills.Count < 8 * (page + 1)) {
    //             for (int i = 0; i < monster.skills.Count - 8 * page; i++) {
    //                 items[i].text = monster.skills[i + 8 * page].param.name_ja;
    //                 items[i].enabled = true;
    //             }
    //             for (int i = monster.skills.Count - 8 * page; i < 8; i++) {
    //                 items[i].enabled = false;
    //             }
    //             toRight.GetComponent<RawImage>().enabled = false;
    //         } 
    //         // 次のページが必要な場合
    //         else {
    //             for (int i = 0; i < 8; i++) {
    //                 items[i].text = monster.skills[i + 8 * page].param.name_ja;
    //                 items[i].enabled = true;
    //             }
    //         }
    //     }
    // }

    // public bool CloseWindow(Vector3 mousePos) 
    // {
    //     Vector3 relativeMousePos = escape_cross.transform.InverseTransformPoint(mousePos);
    //     Vector2 escape_cross_size = escape_cross.GetComponent<RectTransform>().sizeDelta;
    //     if ((relativeMousePos.x >= -(escape_cross_size.x / 2) && relativeMousePos.x <= escape_cross_size.x / 2) &&
    //     (relativeMousePos.y >= -(escape_cross_size.y / 2) && relativeMousePos.y <= escape_cross_size.y / 2)
    //     ) {
    //         canvas.gameObject.SetActive(false);
    //         return true;
    //     }
    //     return false;
    // }

    // public int SelectSkills(Vector3 mousePos)
    // {
    //     // 選ぶ選択肢がない場合
    //     if (items[0].enabled == false) {
    //         return -1;
    //     }
    //     Vector2 item_block_size = items[0].gameObject.GetComponent<RectTransform>().sizeDelta;
    //     // Vector3 relativeMousePos = escape_cross.transform.InverseTransformPoint(mousePos);
    //     Vector3 relativeMousePos;
    //     for (int i = 0; i < items.Count; i++) {
    //         if (items[i].enabled == false) {
    //             continue;
    //         }
    //         relativeMousePos = items[i].transform.InverseTransformPoint(mousePos);
    //         if ((relativeMousePos.x >= -(item_block_size.x / 2) && relativeMousePos.x <= item_block_size.x / 2) &&
    //         (relativeMousePos.y >= -(item_block_size.y / 2) && relativeMousePos.y <= item_block_size.y / 2)
    //         ) {
    //             canvas.gameObject.SetActive(false);
    //             return 8*page + i;
    //         }
    //     }
    //     return -1;
        
    // }
}