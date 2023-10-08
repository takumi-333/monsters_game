using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    
    GameObject Screen;
    Vector2 screen_size;
    private double _time;
    private Animator anim;
    GameObject FadeCurtain;
    // Start is called before the first frame update
    void Start()
    {
        Screen = GameObject.Find("TitleImage");
        FadeCurtain = GameObject.Find("FadeCurtain");
        anim = FadeCurtain.GetComponent<Animator>();
        screen_size = Screen.GetComponent<RectTransform>().sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        // StartCoroutine("Sample");
        if ((Input.GetKey (KeyCode.Return) || Input.GetKey (KeyCode.Space) || Input.GetMouseButtonDown(0)) && _time >= 2.5f) {
            StartCoroutine("FadeAnimation");
        }
    }

    void Change2BattleScene() {
        SceneManager.LoadScene("BattleScene");
    }

    private IEnumerator FadeAnimation() {
        anim.SetBool("fadeOut", true);
        yield return new WaitForSeconds (2f);
        Change2BattleScene();
    }
    
}
