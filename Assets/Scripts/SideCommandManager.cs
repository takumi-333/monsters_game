using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideCommandManager
{
    private Canvas canvas;
    private GameObject speed_wrapper;
    private Image speed1, speed2, speed3;
    public float battle_speed;

    public SideCommandManager(Canvas canvas) {
        this.canvas = canvas;
        speed_wrapper = canvas.transform.Find("SpeedWrapper").gameObject;
        speed1 = speed_wrapper.transform.Find("Speed1").GetComponent<Image>();
        speed2 = speed_wrapper.transform.Find("Speed2").GetComponent<Image>();
        speed3 = speed_wrapper.transform.Find("Speed3").GetComponent<Image>();
        speed2.enabled = false;
        speed3.enabled = false;
        battle_speed = 1.0f;
    }

    public void HandleClickDoubleSpeeder(Vector3 mousePos) {
        if (speed_wrapper.activeSelf == false) return;
        Rect speed_wrapper_size = speed_wrapper.GetComponent<RectTransform>().rect;
        Vector3 relativeMousePos = speed_wrapper.transform.InverseTransformPoint(mousePos);
        if ((relativeMousePos.x >= speed_wrapper_size.xMin && relativeMousePos.x <= speed_wrapper_size.xMax) &&
            (relativeMousePos.y >= speed_wrapper_size.yMin && relativeMousePos.y <= speed_wrapper_size.yMax)) 
        {
            if (battle_speed == 1.0f) {
                battle_speed = 2.0f;
                speed2.enabled = true;
            } else if (battle_speed == 2.0f) {
                battle_speed = 3.0f;
                speed3.enabled = true;
            } else {
                battle_speed = 1.0f;
                speed2.enabled = false;
                speed3.enabled = false;
            }
        }
    }

    public void SetSpeederActive(bool b) {
        speed_wrapper.SetActive(b);
    }
}
