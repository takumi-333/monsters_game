using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmEventManager : MonoBehaviour
{
    private FarmCanvasManager FCM;
    private MapManager MM;
    private FarmDataManager FDM;
    private FarmData farm_data;
    private SaveDataManager SDM;
    public SaveMonsterData load_save_data;
    // Start is called before the first frame update
    void Start()
    {

        MM = GameObject.FindWithTag("MapManager").GetComponent<MapManager>();
        // 牧場データの取得
        FDM = new FarmDataManager();
        farm_data = FDM.Load();
        if (farm_data == null) {
            farm_data = new FarmData();
        }
        // 牧場リストを表示
        FCM = new FarmCanvasManager(GameObject.Find("FarmCanvas").GetComponent<Canvas>(), MM.player_monsters, farm_data);
        FCM.OpenFarmCanvas();

        SDM = new SaveDataManager();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos;
        mousePos = Input.mousePosition;
        FCM.MoveWindow(mousePos);
        if (Input.GetMouseButtonDown(0)) {
            mousePos = Input.mousePosition;
            // farm event finish
            FCM.HoldWindow(mousePos);
            if (FCM.ClickCloseButton(mousePos)) {
                // 終了時にセーブをする
                FDM.Save(farm_data);
                load_save_data = new SaveMonsterData(MM.player_monsters);
                load_save_data.SetMonsterData(MM.player_monsters);
                SDM.Save(load_save_data);

                MM.PC.can_move = true;
                MM.PC.StartWalking();
                Destroy(this);
            }
            FCM.ClickNextButton(mousePos);
            FCM.ClickPrevButton(mousePos);
        }
        if (Input.GetMouseButtonUp(0)) {
            mousePos = Input.mousePosition;
            FCM.ReleaseWindow(mousePos);
        }
    }
}
