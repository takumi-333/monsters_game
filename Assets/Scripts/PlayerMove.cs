using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    private Animator animator;
    public Direction direction;
    private float steps;
    private float encount_steps;
    public float min_encount_steps;
    public float max_encout_steps;

    private double _time;
    private Vector3 current_pos, next_pos;
    public float input_interval_time = 0.1f;

    private float min_x;
    private float min_y;
    private float max_x;
    private float max_y;

    private Monster monster_tmp;
    private MonsterData monster_data;

    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }

    private bool OutMap(Vector3 pos) {
        if (pos.x < min_x || pos.x > max_x || pos.y < min_y || pos.y > max_y) {
            return true;
        } else {
            return false;
        }
    }

    private void NextScene()
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
            gameManager.test_monster = monster_tmp; 

            // イベントからメソッドを削除
            SceneManager.sceneLoaded -= GameSceneLoaded;
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        direction = Direction.DOWN;
        steps = 0;
        encount_steps = Random.Range(min_encount_steps, max_encout_steps);
        _time = 0;
        current_pos = transform.position;

        monster_data = Resources.Load("monster_data") as MonsterData;
        monster_tmp = new Monster(monster_data.sheets[0].list.Find(monster=> monster.id == 5));

        min_x = -7.5f;
        min_y = -3.5f;
        max_x = 7.5f;
        max_y = 3.5f;
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
                NextScene();
            }
        }
    }
}
    


