using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public enum SkillType 
    {
        PHYSICAL,
        MAGIC,
        HEAL,
    }

    public enum Element
    {
        NONE,
        FIRE,
        EARTH,
        AIR,
        WATER,
    }

    public SkillType skillType;
    public Element eType;
    public SkillData.Param param;

    // コンストラクタ
    public Skill(SkillData.Param param) 
    {
        this.param =  param.ShallowCopy();
        switch(param.type) {
            case 1:
                skillType = SkillType.PHYSICAL;
                break;
            case 2:
                skillType = SkillType.MAGIC;
                break;
            case 3:
                skillType = SkillType.HEAL;
                break;
        }
        switch(param.element)
        {
            case 0:
                eType = Element.NONE;
                break;
            case 1:
                eType = Element.FIRE;
                break;
            case 2:
                eType = Element.EARTH;
                break;
            case 3:
                eType = Element.AIR;
                break;
            case 4:
                eType = Element.WATER;
                break;
        }
    }

    
}
