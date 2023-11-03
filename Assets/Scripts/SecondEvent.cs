using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SecondEvent : MonoBehaviour
{
    private List<PlayerMonster> default_player_monsters;
    public string message1 = "ふっふっふ、これで私の...";
    public string message2 = "(生き残りのオクパと鍛えて出直そう)";
    private MapManager MM;
    private MessageWindowManager MWM;
    private MonsterData monster_data;
    private EachMonsterData each_monster_data;
    private bool talking;
    // Start is called before the first frame update
    void Start()
    {
        monster_data = Resources.Load("monster_data") as MonsterData;
        each_monster_data = Resources.Load("each_monster_data") as EachMonsterData;

        default_player_monsters = new List<PlayerMonster>();
        SetDefaultPlayerMonster();
        MM = gameObject.GetComponent<MapManager>();
        MM.PC.can_move = false;
        MM.PC.direction = PlayerController.Direction.UP;
        MM.PC.StopWalking();
        MWM = new MessageWindowManager(GameObject.FindWithTag("MessageWindowCanvas").GetComponent<Canvas>());
        MWM.DisplayCanvas();
        talking = true;
        StartCoroutine("DisplayMessage");
    }

    public void SetDefaultPlayerMonster()
    {
        int[] id = {1};
        int[] level = {3};
        string[] name = {"オクパ"};
        PlayerMonster pmon;
        for (int i = 0; i < 1; i++) {
            MonsterData.Param param = monster_data.sheets[0].list.Find(param=>param.id == id[i]);
            EachMonsterData.Param u_param = each_monster_data.sheets.Find(sheet=>sheet.name==id[i].ToString()).list.Find(param=>param.lv==level[i]);
            pmon = new PlayerMonster(u_param, param);
            pmon.name_ja = name[i];
            default_player_monsters.Add(pmon);
        }
    }

    private IEnumerator DisplayMessage()
    {
        StartCoroutine(MWM.DisplayMessage1(message1));
        yield return new WaitUntil(() => !MWM.displaying);
        StartCoroutine(MWM.DisplayMessage2(message2));
        yield return new WaitUntil(() => !MWM.displaying);
        yield return new WaitForSeconds(1.0f);
        talking = false;
        MWM.ClearAll();
        // MM.PC.can_move = true;
        // MM.PC.StartWalking();
        PlayerPrefs.SetInt("Event1", 0);
        PlayerPrefs.SetString("MapName", "Map3Scene");
        MoveMapScene();
    }

    public void MoveMapScene()
    {
        SceneManager.sceneLoaded += GameSceneLoaded;

        SceneManager.LoadScene("Map3Scene");

        void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            PlayerController pc = 
            GameObject.FindWithTag("Player").GetComponent<PlayerController>();

            MapManager MM =
                GameObject.FindWithTag("MapManager").GetComponent<MapManager>();
            // AreaDoor next_door = 
            //     GameObject.Find(area_door.next_area_door).GetComponent<AreaDoor>();
            
            MM.steps = 0;
            MM.player_monsters = default_player_monsters;
            MM.PC = pc;
            // MM.area_door = next_door;
            SceneManager.sceneLoaded -= GameSceneLoaded;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
