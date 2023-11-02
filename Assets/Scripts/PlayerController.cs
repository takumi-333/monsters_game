using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    public Direction direction = Direction.DOWN;
    public float speed;

    public bool can_move = false;

    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }

    public void StopWalking()
    {
        animator.speed = 0;
    }

    public void StartWalking()
    {
        animator.speed = 1;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("direction", (int)direction);
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.D)) Debug.Log(can_move);
        if (can_move) {
            Vector3 pos = transform.position;
            if (Input.GetKey(KeyCode.UpArrow))//↑キーを押したら
            {
                direction = Direction.UP;
                animator.SetInteger("direction",(int)direction);
                pos.y += speed;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                direction = Direction.RIGHT;
                animator.SetInteger("direction", (int)direction);
                pos.x += speed;
            }
            else if (Input.GetKey(KeyCode.DownArrow))//↓キーを押したら
            {
                direction = Direction.DOWN;
                animator.SetInteger("direction", (int)direction);
                pos.y -= speed;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))//←キーを押したら
            {
                direction = Direction.LEFT;
                animator.SetInteger("direction", (int)direction);
                pos.x -= speed;
            }
            transform.position = pos;
        }
    }
}
    


