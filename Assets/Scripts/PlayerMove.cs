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
    public int[] player_monster_id_list = {0, 1, 2, 6};

    private double _time;
    private Vector3 current_pos, next_pos;
    public float input_interval_time = 0.1f;

    // mapの領域（応急処置）
    private float min_x;
    private float min_y;
    private float max_x;
    private float max_y;


    public int num_player_monster;
    public List<PlayerMonster> player_monsters;
    private MonsterData monster_data;
    private SkillData skill_data;

    private List<List<int>> player_monster_skill_ids;

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
        if(player_monsters != null) {
            num_player_monster = player_monsters.Count;
        } else {
            num_player_monster = player_monster_id_list.Length;
        }
        if (player_monsters == null) {
            player_monsters = new List<PlayerMonster>();
            // 仮の形式
            player_monster_skill_ids = new List<List<int>>();
            List<int> player_monster1_skill_ids = new List<int>(){1,2,3};
            player_monster_skill_ids.Add(player_monster1_skill_ids);
            List<int> player_monster2_skill_ids = new List<int>(){1,2,3,4,5,6,7,8,9,10,11,12,13};
            player_monster_skill_ids.Add(player_monster2_skill_ids);
            List<int> player_monster3_skill_ids = new List<int>(){1,2,5,6,7};
            player_monster_skill_ids.Add(player_monster3_skill_ids);
            List<int> player_monster4_skill_ids = new List<int>(){8,9,13};
            player_monster_skill_ids.Add(player_monster4_skill_ids);
            SetPlayerMonsters();
        }
        min_x = -7.5f;
        min_y = -3.5f;
        max_x = 7.5f;
        max_y = 3.5f;
    }

    private void SetPlayerMonsters() 
    {
        MonsterData.Param player_monster_param;
        PlayerMonster monster;
        for (int i = 0; i < num_player_monster; i++) {
            player_monster_param = monster_data.sheets[0].list.Find(monster=> monster.id == player_monster_id_list[i]);
            monster = new PlayerMonster(player_monster_param);
            List<Skill> skills = new List<Skill>();
            for (int j = 0; j < player_monster_skill_ids[i].Count; j++)
            {
                Skill add_skill = new Skill(skill_data.sheets[0].list.Find(skill=> skill.id  == player_monster_skill_ids[i][j]));
            }
            monster.SetSkills(skills);
            player_monsters.Add(monster);
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
                GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

            // データを渡す処理
            gameManager.player_monsters = player_monsters;

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
    


