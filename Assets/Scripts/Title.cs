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
    // Start is called before the first frame update
    void Start()
    {
        Screen = GameObject.Find("TitleImage");
        FadeCurtain = GameObject.Find("FadeCurtain");
        anim = FadeCurtain.GetComponent<Animator>();
        screen_size = Screen.GetComponent<RectTransform>().sizeDelta;
        audio_source = GetComponent<AudioSource>();
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

    void CallMapScene() {
        SceneManager.LoadScene("MapScene");
    }

    private IEnumerator FadeAnimation() {
        anim.SetBool("fadeOut", true);
        yield return new WaitForSeconds (2f);
        CallMapScene();
    }
    
}
