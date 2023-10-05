using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonsterData.Param
{
    public bool isEnemy = false;
    public MonsterData.Param param;
    private Action action;
    private RawImage image;
    private GameObject status_window;

    public Monster(MonsterData.Param param) {
        this.param =  param.ShallowCopy();
    }

    public void SetAction(Action action) {
        this.action = action;
    }

    public Action GetAction() {
        return action;
    }

    public void SetImage(RawImage image) {
        this.image = image;
    }

    public RawImage GetImage() {
        return image;
    }

    public bool isDead() {
        if (param.hp <= 0) {
            return true;
        }
        return false;
    }

    public void SetStatusWindow(GameObject status_window) {
        this.status_window = status_window;
    }

    public GameObject GetStatusWindow() {
        if (isEnemy) {
            return null;
        } else {
            return status_window;
        }
    }
}
