using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AreaDoor : MonoBehaviour
{
    public string next_area_scene = "Map2Scene";
    public string next_area_door = "AreaDoor";
    public float wait_time = 1.0f;
    public bool fading = false;
    private Image fade_curtain;
    private MapManager MM;
    public PlayerController.Direction next_direction;
    // public string areaToLoad;
    // public string areaTransitionName;
    // public AreaEntrance theEntrance;
    // private bool shouldLoadAfterFade;
    // Start is called before the first frame update
    void Start()
    {
        fade_curtain = GameObject.Find("FadeCurtain").GetComponent<Image>();
        MM = GameObject.FindWithTag("MapManager").GetComponent<MapManager>();
        // theEntrance.transitionName = areaTransitionName;
    }
    // Update is called once per frame
    void Update()
    {
        // if(shouldLoadAfterFade)
        // {
        //     waitToLoad -= Time.deltaTime;
        //     if(waitToLoad <= 0){
        //         shouldLoadAfterFade = false;
        //         SceneManager.LoadScene(areaToLoad);
        //     }
        // }
    }

    public IEnumerator fade_out() {
        if (fading == true) yield break;
        fading = true;
        int loop_count = 10;
        float time = wait_time / loop_count;
        float delta_alpha = 1.0f / loop_count;
        for (int i = 0; i < loop_count; i++){
            yield return new WaitForSeconds(time);
            Color color = fade_curtain.color;
            color.a += delta_alpha;
            fade_curtain.color = color;
        }
        fading = false;
        MM.MoveMapScene();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {   
            MM.area_door = this;
            StartCoroutine("fade_out");
            // shouldLoadAfterFade = true;
            // GameManager.instance.fadingBetweenAreas = true;
            // UIFade.instance.FadeToBlack();
            // PlayerController.instance.areaTransitionName = areaTransitionName;
        }
    }
}