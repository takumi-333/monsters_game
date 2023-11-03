using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuCanvasManager
{
    private Canvas canvas;
    private GameObject save;
    private GameObject menu;
    private GameObject status;
    private GameObject saved;
    private TextMeshProUGUI saved_message;
    private SaveMonsterData save_data;
    private SaveDataManager SDM;
    private bool saving = false;
    
    public MenuCanvasManager(Canvas canvas)
    {
        this.canvas = canvas;
        save = canvas.transform.Find("SaveButton").gameObject;
        menu = canvas.transform.Find("MenuButton").gameObject;
        status = canvas.transform.Find("StatusButton").gameObject;
        saved = canvas.transform.Find("SavedText").gameObject;
        saved_message = saved.transform.Find("Message").GetComponent<TextMeshProUGUI>();
        SDM = new SaveDataManager();
        save.SetActive(false);
        status.SetActive(false);
        saved.SetActive(false);
    }

    public bool ClickSaveButton(Vector3 mousePos)
    {
        if (save.activeSelf == false) return false;
        Rect button_size = save.GetComponent<RectTransform>().rect;
        Vector3 relativeMousePos = save.transform.InverseTransformPoint(mousePos);
        if ((relativeMousePos.x >= button_size.xMin && relativeMousePos.x <= button_size.xMax) &&
            (relativeMousePos.y >= button_size.yMin && relativeMousePos.y <= button_size.yMax)) 
        {
            return true;
        }
        return false;
    }

    public void HandleSave(MapManager MM)
    {
        if (saving) return;
        // 位置
        PlayerPrefs.SetFloat("PosX", MM.player_position.x);
        PlayerPrefs.SetFloat("PosY", MM.player_position.y);
        PlayerPrefs.SetFloat("PosZ", MM.player_position.z);

        // map名
        PlayerPrefs.SetString("MapName", MM.map_scene_name);

        // monster info
        // PlayerPrefs.SetInt("NumMonsters", MM.player_monsters.Count);
        // for (int i  = 0; i < MM.player_monsters.Count; i++) {
        //     PlayerPrefs.SetInt($"MonsterId{i}", MM.player_monster_id_list[i]);
        // }
        save_data = new SaveMonsterData(MM.player_monsters);
        save_data.SetMonsterData(MM.player_monsters);
        SDM.Save(save_data);
    }

    public IEnumerator DisplaySavedMessage()
    {
        if (saving) yield break;
        saving = true;
        saved.SetActive(true);
        string message = saved_message.text;
        saved_message.text = "";
        saved_message.text += message[0];
        for (int i = 1; i < message.Length; i++) {
            yield return new WaitForSeconds (0.1f);
            saved_message.text += message[i];
        }
        yield return new WaitForSeconds (0.5f);
        saved.SetActive(false);
        saving = false;
    }

    public bool ClickMenuButton(Vector3 mousePos)
    {
        if (menu.activeSelf == false) return false;
        Rect button_size = menu.GetComponent<RectTransform>().rect;
        Vector3 relativeMousePos = menu.transform.InverseTransformPoint(mousePos);
        if ((relativeMousePos.x >= button_size.xMin && relativeMousePos.x <= button_size.xMax) &&
            (relativeMousePos.y >= button_size.yMin && relativeMousePos.y <= button_size.yMax)) 
        {
            return true;
        }
        return false;
    }

    public void HandleMenu()
    {
        status.SetActive(!status.activeSelf);
        save.SetActive(!save.activeSelf);
    }

    public bool ClickStatusButton(Vector3 mousePos)
    {
        if (status.activeSelf == false) return false;
        Rect button_size = status.GetComponent<RectTransform>().rect;
        Vector3 relativeMousePos = status.transform.InverseTransformPoint(mousePos);
        if ((relativeMousePos.x >= button_size.xMin && relativeMousePos.x <= button_size.xMax) &&
            (relativeMousePos.y >= button_size.yMin && relativeMousePos.y <= button_size.yMax)) 
        {
            return true;
        }
        return false;
    }

    public SaveMonsterData GetSaveData()
    {
        return SDM.Load();
    }

}
