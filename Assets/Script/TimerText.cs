using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TimerText : MonoBehaviour
{
    public GameObject TimerHp;
    Timer THp;
    public int intHP;

    TextMeshProUGUI Ttext;

    // Start is called before the first frame update
    void Start()
    {
        THp = TimerHp.GetComponent<Timer>();
        Ttext = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        intHP = (int)THp.Hp;
        Ttext.text = intHP.ToString();
    }
}
