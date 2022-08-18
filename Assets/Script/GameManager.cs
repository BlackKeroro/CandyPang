using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject BackGround;
    public List<Sprite> spr;
    public GameObject[] Audio;
    SpriteRenderer sr;
    AudioSource ad;

    public GameObject Pause;

    // Start is called before the first frame update
    void Start()
    {
        sr = BackGround.GetComponent<SpriteRenderer>();
        sr.sprite = spr[GridManager.Stage];
        ad = Audio[GridManager.Stage].GetComponent<AudioSource>();
        ad.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Time.timeScale = 0;
            Pause.gameObject.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }
    public void Home()
    {
        SceneManager.LoadScene("Stage");
        Time.timeScale = 1;
    }
    public void Play() 
    {
        Pause.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
   
}
