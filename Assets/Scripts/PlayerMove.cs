using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Animator animator;

    public Direction direction;
    private Vector3 current_pos, next_pos;
    private double _time;
    private float input_reception_time;

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
        current_pos = transform.position;
        direction = Direction.DOWN;
        _time = 0;
        input_reception_time = 0.25f;
    }

    void Update()
    {
        _time += Time.deltaTime;
        if (_time >= input_reception_time) {
            if (Input.GetKey(KeyCode.UpArrow))//↑キーを押したら
            {
                direction = Direction.UP;
                animator.SetInteger("direction",(int)direction);
                next_pos = current_pos + new Vector3(0,1,0);
                transform.position = next_pos;
                current_pos = transform.position;
                _time = 0;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                direction = Direction.RIGHT;
                animator.SetInteger("direction", (int)direction);
                next_pos = current_pos + new Vector3(1,0,0);
                transform.position = next_pos;
                current_pos = transform.position;
                _time = 0;
            }
            else if (Input.GetKey(KeyCode.DownArrow))//↓キーを押したら
            {
                direction = Direction.DOWN;
                animator.SetInteger("direction", (int)direction);
                next_pos = current_pos + new Vector3(0,-1,0);
                transform.position = next_pos;
                current_pos = transform.position;
                _time = 0;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))//←キーを押したら
            {
                direction = Direction.LEFT;
                animator.SetInteger("direction", (int)direction);
                next_pos = current_pos + new Vector3(-1,0,0);
                transform.position = next_pos;
                current_pos = transform.position;
                _time = 0;
            }
        }
    }

    
}
    


