using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    private Animator animator;
    private Direction direction;
    private float steps;
    private float encount_steps;

    public float min_encount_steps;
    public float max_encout_steps;
    public int[] pMonster_id_list = {0, 1, 2, 6};

    private double _time;
    private Vector3 current_pos, next_pos;
    public float input_interval_time = 0.1f;

    private float min_x;
    private float min_y;
    private float max_x;
    private float max_y;


    public int num_pMonster;
    public List<Monster> pMonsters;
    private MonsterData monster_data;
    private SkillData skill_data;

    private List<List<int>> pMonster_skill_ids;

    private AudioSource audio_source;
    // public AudioClip map_bgm;
    public AudioClip encount_enemy_sound;


    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        audio_source = GetComponent<AudioSource>();
        direction = Direction.DOWN;
        steps = 0;
        encount_steps = Random.Range(min_encount_steps, max_encout_steps);
        _time = 0;
        current_pos = transform.position;
        audio_source.Play();

        monster_data = Resources.Load("monster_data") as MonsterData;
        skill_data = Resources.Load("skill_data") as SkillData;
        if(pMonsters != null) {
            num_pMonster = pMonsters.Count;
        } else {
            num_pMonster = pMonster_id_list.Length;
        }
        if (pMonsters == null) {
            pMonsters = new List<Monster>();
            // 仮の形式
            pMonster_skill_ids = new List<List<int>>();
            List<int> pMonster1_skill_ids = new List<int>(){1,2,3};
            pMonster_skill_ids.Add(pMonster1_skill_ids);
            List<int> pMonster2_skill_ids = new List<int>(){1,2,3,4,5,6,7,8,9,10,11,12,13};
            pMonster_skill_ids.Add(pMonster2_skill_ids);
            List<int> pMonster3_skill_ids = new List<int>(){1,2,5,6,7};
            pMonster_skill_ids.Add(pMonster3_skill_ids);
            List<int> pMonster4_skill_ids = new List<int>(){8,9,13};
            pMonster_skill_ids.Add(pMonster4_skill_ids);
            SetPlayerMonsters();
        }
        min_x = -7.5f;
        min_y = -3.5f;
        max_x = 7.5f;
        max_y = 3.5f;
    }

    private void SetPlayerMonsters() 
    {
        MonsterData.Param pMonster_param;
        Monster monster;
        for (int i = 0; i < num_pMonster; i++) {
            pMonster_param = monster_data.sheets[0].list.Find(monster=> monster.id == pMonster_id_list[i]);
            monster = new Monster(pMonster_param);
            monster.SetSkills(pMonster_skill_ids[i], skill_data);
            pMonsters.Add(monster);
        }
    }

    private bool OutMap(Vector3 pos) {
        if (pos.x < min_x || pos.x > max_x || pos.y < min_y || pos.y > max_y) {
            return true;
        } else {
            return false;
        }
    }

    private void CallBattleScene()
    {
            // イベントにメソッドを登録
            SceneManager.sceneLoaded += GameSceneLoaded;

            SceneManager.LoadScene("BattleScene");

        void GameSceneLoaded(Scene next, LoadSceneMode mode)
        {
            // シーン切り替え後のスクリプトを取得
            var gameManager = 
                GameObject.FindWithTag("BattleManager").GetComponent<BattleManager>();

            // データを渡す処理
            gameManager.num_pMonster = num_pMonster;
            gameManager.pMonsters = pMonsters;

            // イベントからメソッドを削除
            SceneManager.sceneLoaded -= GameSceneLoaded;
        }
    }

    private IEnumerator TransBattleScene() {
        audio_source.Stop();
        audio_source.PlayOneShot(encount_enemy_sound);
        yield return new WaitForSeconds (1.0f);
        CallBattleScene();
    }
    

    void Update()
    {
        current_pos = transform.position;
        _time += Time.deltaTime;
        if (_time >= input_interval_time) {
            if (Input.GetKey(KeyCode.UpArrow))//↑キーを押したら
            {
                direction = Direction.UP;
                animator.SetInteger("direction",(int)direction);
                next_pos = current_pos + new Vector3(0,1,0);
                if (!OutMap(next_pos)) {
                    transform.position = next_pos;
                    steps += 1;
                }
                _time = 0;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                direction = Direction.RIGHT;
                animator.SetInteger("direction", (int)direction);
                next_pos = current_pos + new Vector3(1,0,0);
                if (!OutMap(next_pos)) {
                    transform.position = next_pos;
                    steps += 1;
                }
                _time = 0;
            }
            else if (Input.GetKey(KeyCode.DownArrow))//↓キーを押したら
            {
                direction = Direction.DOWN;
                animator.SetInteger("direction", (int)direction);
                next_pos = current_pos + new Vector3(0,-1,0);
                if (!OutMap(next_pos)) {
                    transform.position = next_pos;
                    steps += 1;
                }
                _time = 0;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))//←キーを押したら
            {
                direction = Direction.LEFT;
                animator.SetInteger("direction", (int)direction);
                next_pos = current_pos + new Vector3(-1,0,0);
                if (!OutMap(next_pos)) {
                    transform.position = next_pos;
                    steps += 1;
                }
                _time = 0;
            }
            if (steps >= encount_steps) {
                Debug.Log("Encount!!");
                encount_steps = Random.Range(min_encount_steps, max_encout_steps);
                steps = 0;
                StartCoroutine("TransBattleScene");
                _time = -100f;
            }
        }
    }
}
    


