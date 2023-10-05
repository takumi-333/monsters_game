using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonsterData.Param
{
    public MonsterData.Param param;
    private Action action;
    private RawImage image;

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
}
