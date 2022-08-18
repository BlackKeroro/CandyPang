using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public List<Sprite> spr;
    public GameObject[] Audio;
    SpriteRenderer sr;
    AudioSource ad;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = spr[GridManager.Stage];
        ad = Audio[GridManager.Stage].GetComponent<AudioSource>();
        ad.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
