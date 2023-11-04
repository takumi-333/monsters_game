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
    private TextMeshProUGUI tame_yes_message;
    private TextMeshProUGUI tame_no_message;
    public List<Image> tame_cursors;
    public Image focus_cursor;
    private List<GameObject> commands;
    public bool displaying;
    private string[] command_str;
    public bool taming;

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
        tame_yes_message = command_window.transform.Find("TameYes").gameObject.GetComponent<TextMeshProUGUI>();
        tame_no_message = command_window.transform.Find("TameNo").gameObject.GetComponent<TextMeshProUGUI>();
        tame_cursors = new List<Image>();
        tame_cursors.Add(tame_yes_message.transform.Find("Cursor").gameObject.GetComponent<Image>());
        tame_cursors.Add(tame_no_message.transform.Find("Cursor").gameObject.GetComponent<Image>());
        
        battle_message1.enabled = false;
        battle_message2.enabled = false;
        tame_yes_message.enabled = false;
        tame_no_message.enabled = false;
        foreach(Image cursor in tame_cursors) {
            cursor.enabled = false;
        }
        displaying = false;
        command_str = new string[]{"こうげき", "どうぐ", "とくぎ", "にげる"};
        commands = new List<GameObject>();
        for (int i = 0; i < 4; i++) {
            GameObject command = command_window.transform.Find($"Command{i}").gameObject;
            command.transform.Find("CommandText").GetComponent<TextMeshProUGUI>().text = command_str[i];
            command.SetActive(false);
            commands.Add(command);
        }

        taming = false;
        focus_cursor = null;
    }
    
    public void SetBattleMessage1(string str)
    {
        Debug.Log(str + "を表示する");
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

    public void DisplayTameMessage()
    {
        battle_message2.enabled = false;
        tame_yes_message.enabled = true;
        tame_no_message.enabled = true;
    }

    public void SelectTameMessage(Vector3 mousePos) {
        Rect message_size;
        Vector3 relativeMousePos;
        if (tame_yes_message.enabled) {
            message_size = tame_yes_message.gameObject.GetComponent<RectTransform>().rect;
            relativeMousePos = tame_yes_message.transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= message_size.xMin && relativeMousePos.x <= message_size.xMax) &&
                (relativeMousePos.y >= message_size.yMin && relativeMousePos.y <= message_size.yMax)) {
                focus_cursor = tame_cursors[0];
                return;
            }
        }
        if (tame_no_message.enabled) {
            message_size = tame_no_message.gameObject.GetComponent<RectTransform>().rect;
            relativeMousePos = tame_no_message.transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= message_size.xMin && relativeMousePos.x <= message_size.xMax) &&
                (relativeMousePos.y >= message_size.yMin && relativeMousePos.y <= message_size.yMax)) {
                focus_cursor = tame_cursors[1];
                return;
            }
        }
        focus_cursor = null;
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
}