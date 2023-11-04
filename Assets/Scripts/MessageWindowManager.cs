using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageWindowManager
{
    private Canvas canvas;
    private GameObject message_window;
    private GameObject select_window;
    private TextMeshProUGUI message1;
    private TextMeshProUGUI message2;
    private List<TextMeshProUGUI> select_messages;
    private List<Image> select_cursors;
    public Image focus_cursor;
    public bool select_answer;

    public bool skip;

    public bool displaying;
    
    public MessageWindowManager(Canvas canvas)
    {
        this.canvas = canvas;
        message_window = canvas.transform.Find("MessageWindow").gameObject;
        select_window = canvas.transform.Find("SelectWindow").gameObject;
        message1 = message_window.transform.Find("Message1").gameObject.GetComponent<TextMeshProUGUI>();
        message2 = message_window.transform.Find("Message2").gameObject.GetComponent<TextMeshProUGUI>();
        select_messages = new List<TextMeshProUGUI>();
        select_cursors = new List<Image>();
        select_messages.Add(select_window.transform.Find("YesMessage").gameObject.GetComponent<TextMeshProUGUI>());
        select_messages.Add(select_window.transform.Find("NoMessage").gameObject.GetComponent<TextMeshProUGUI>());
        select_cursors.Add(select_window.transform.Find("YesCursor").gameObject.GetComponent<Image>());
        select_cursors.Add(select_window.transform.Find("NoCursor").gameObject.GetComponent<Image>());
        message1.enabled = false;
        message2.enabled = false;
        foreach(Image cursor in select_cursors) {
            cursor.enabled = false;
        }
        select_window.SetActive(false);
        canvas.enabled = false;
        skip = false;
        displaying = false;
        focus_cursor = select_cursors[0];
    }

    public void DisplayCanvas()
    {
        canvas.enabled = true;
    }

    public void SetMessage1(string message1)
    {
        this.message1.text = message1;
    }

    public void SetMessage2(string message2)
    {
        this.message2.text = message2;
    }

    public void ClearAllMessage()
    {
        message1.text = "";
        message2.text = "";
        message1.enabled = false;
        message2.enabled = false;
    }

    public IEnumerator DisplayMessage1(string message) 
    {
        if (displaying) yield break;
        displaying = true;
        message1.text = "";
        message1.enabled = true;
        message1.text += message[0];
        for (int i = 1; i < message.Length; i++) {
            message1.text += message[i];
            if (!skip) yield return new WaitForSeconds (0.1f);
            else yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.25f);
        displaying = false;
        skip = false;
    }

    public IEnumerator DisplayMessage2(string message) 
    {
        Debug.Log("2" + displaying);
        if (displaying) yield break;
        displaying = true;
        message2.text = "";
        message2.enabled = true;
        message2.text += message[0];
        for (int i = 1; i < message.Length; i++) {
            message2.text += message[i];
            if (!skip) yield return new WaitForSeconds (0.1f);
            else yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds (0.25f);
        displaying = false;
        skip = false;
    }

    public void DisplaySelectWindow()
    {
        select_window.SetActive(true);
        Debug.Log(focus_cursor.enabled);
    }

    public bool SelectMessage(Vector3 mousePos) {
        Rect message_size;
        Vector3 relativeMousePos;
        if (select_window.activeSelf == false) return false;
        for(int i = 0; i < select_messages.Count; i++) {
            message_size = select_messages[i].gameObject.GetComponent<RectTransform>().rect;
            relativeMousePos = select_messages[i].transform.InverseTransformPoint(mousePos);
            if ((relativeMousePos.x >= message_size.xMin && relativeMousePos.x <= message_size.xMax) &&
                (relativeMousePos.y >= message_size.yMin && relativeMousePos.y <= message_size.yMax)) {
                focus_cursor = select_cursors[i];
                if (i == 0) {
                    select_cursors[1].enabled = false;
                    select_answer = true;
                }
                else {
                    select_cursors[0].enabled = false;
                    select_answer = false;
                }
                return true;
            }
        }
        return false;
    }

    public void ToggleAnswer()
    {
        // 元々yesだった場合
        if (focus_cursor == select_cursors[0]) {
            focus_cursor.enabled = false;
            focus_cursor = select_cursors[1];
            select_answer = false;
        }
        else if (focus_cursor == select_cursors[1]) {
            focus_cursor.enabled = false;
            focus_cursor = select_cursors[0];
            select_answer = true;
        }
        else {
            Debug.LogError("Error: answer is invalid");
        }
    }

    public void SelectYesAnswer()
    {
        select_cursors[1].enabled = false;
        focus_cursor = select_cursors[0];
        select_answer = true;
    }

    public void SelectNoAnswer()
    {
        select_cursors[0].enabled = false;
        focus_cursor = select_cursors[1];
        select_answer = false;
    }
    public void ClearAll()
    {
        select_window.SetActive(false);
        message1.enabled = false;
        message2.enabled = false;
        skip = false;
        displaying = false;
        focus_cursor = select_cursors[0];
        canvas.enabled = false;
    }

}
