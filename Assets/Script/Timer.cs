using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    Image Hpbar;
    public float Hp;
    public float MaxHp;

    public GameObject UICanvas;
    // Start is called before the first frame update
    public void Start()
    {
        Hp = 60f/(GridManager.Stage + 1);
        MaxHp = Hp;
        Hpbar = GetComponent<Image>();
    }

    // Update is called once per frame
    public void Update()
    {
        if(Hp >= 0)
        {
            Hp -= Time.deltaTime;

        }
        Hpbar.fillAmount = Hp / MaxHp;

        if(Hp <= 0)
        {
            UICanvas.SetActive(true);
        }
    }
}
