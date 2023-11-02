using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TalkEvent : MonoBehaviour
{
    public string message1 = "今日も牧場のモンスターたちは元気じゃ";
    public string message2 = "モンスターの入れ替えをするかい？";
    public bool isQuestion = true;

    private MapManager MM;
    private MessageWindowManager MWM;
    private FarmEventManager FEM;
    private bool can_talk;
    private bool talking;
    private bool answering;
    private bool blinking;


    void Start()
    {
        MM = GameObject.FindWithTag("MapManager").GetComponent<MapManager>();
        MWM = new MessageWindowManager(GameObject.FindWithTag("MessageWindowCanvas").GetComponent<Canvas>());
        can_talk = false;
        talking = false;
        answering = false;
        blinking = false;
    }

    private IEnumerator DisplayMessage()
    {
        StartCoroutine(MWM.DisplayMessage1(message1));
        yield return new WaitUntil(() => !MWM.displaying);
        StartCoroutine(MWM.DisplayMessage2(message2));
        yield return new WaitUntil(() => !MWM.displaying);
        if (isQuestion) {
            MWM.DisplaySelectWindow();
            answering = true;
        }
        else {
            talking = false;
            MWM.ClearAll();
            MM.PC.can_move = true;
            MM.PC.StartWalking();
        }
    }

    private IEnumerator BlinkSelectCursor()
    {
        if (blinking) yield break;
        blinking = true;
        while (answering) {
            MWM.focus_cursor.enabled = !MWM.focus_cursor.enabled;
            yield return new WaitForSeconds(0.15f);
        }
    }

    void Update()
    {
        Vector3 mousePos;
        // 人に近づいてspaceを押すことで会話がスタート
        if (can_talk && !talking) {
            if (Input.GetKey(KeyCode.Space)) {
                talking = true;
                // playerの動きを止める
                MM.PC.can_move = false;
                MM.PC.StopWalking();

                MWM.DisplayCanvas();
                StartCoroutine("DisplayMessage");
            }
        }
        // 会話中にマウスクリックで会話を一気に表示
        if (talking) {
            if (Input.GetMouseButtonDown(0)) {
                MWM.skip = true;
            }
        }
        // playerは質問に答える
        if (answering) {
            mousePos = Input.mousePosition;
            // 一回だけ呼ばれる
            StartCoroutine("BlinkSelectCursor");
            MWM.SelectMessage(mousePos);
            if (Input.GetKey(KeyCode.Return) || Input.GetMouseButtonDown(0)) {
                answering = false;
                talking = false;
                // この結果をeventに渡す
                Debug.Log((MWM.select_answer?"Yes":"No") + "が選ばれた");
                MWM.ClearAll();
            }
            if (Input.GetKey(KeyCode.UpArrow)) {
                MWM.SelectYesAnswer();
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                MWM.SelectNoAnswer();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            can_talk = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
     {
        if (other.gameObject.CompareTag("Player"))
        {
            can_talk = false;
        }
    }


}
