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
}
