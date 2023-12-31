using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    public AudioClip click_sound;
    private GameObject Screen;
    private Vector2 screen_size;
    private double _time;
    private Animator anim;
    private GameObject FadeCurtain;
    private AudioSource audio_source;
    private SaveDataManager SDM;
    // Start is called before the first frame update
    void Start()
    {
        Screen = GameObject.Find("TitleImage");
        FadeCurtain = GameObject.Find("FadeCurtain");
        anim = FadeCurtain.GetComponent<Animator>();
        screen_size = Screen.GetComponent<RectTransform>().sizeDelta;
        audio_source = GetComponent<AudioSource>();
        SDM = new SaveDataManager();
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        // StartCoroutine("Sample");
        if ((Input.GetKey (KeyCode.Return) || Input.GetKey (KeyCode.Space) || Input.GetMouseButtonDown(0)) && _time >= 2.5f) {
            audio_source.PlayOneShot(click_sound);
            StartCoroutine("FadeAnimation");
        }
    }
    
    private void CallMapScene()
    {

        // セーブデータの引き出し、Mapシーンへ渡す

        // player monstersの情報
        SaveMonsterData load_data = SDM.Load();
        if (load_data == null) {
            load_data = new SaveMonsterData(null);
        }
        string map_scene_name = load_data.map_name;
        
        Debug.Log(" map" + map_scene_name);
        int event1_flg = load_data.event1_flg;
        SceneManager.sceneLoaded += GameSceneLoaded;
        
        SceneManager.LoadScene(map_scene_name);

        void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            // シーン切り替え後のスクリプトを取得
            var gameManager = 
                GameObject.FindWithTag("MapManager").GetComponent<MapManager>();

            // データを渡す処理
            // gameManager.player_position = pos;
            // gameManager.player_monster_id_list = monster_id_list;
            gameManager.load_data = load_data;
            gameManager.event1_flg = event1_flg;

            SceneManager.sceneLoaded -= GameSceneLoaded;
        }
    }

    private IEnumerator FadeAnimation() {
        anim.SetBool("fadeOut", true);
        yield return new WaitForSeconds (2f);
        CallMapScene();
    }
    
}
