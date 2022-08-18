using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSel : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OneStage()
    {
        GridManager.Stage = 0;
        SceneManager.LoadScene("Game");
    }
    public void TwoStage()
    {
        GridManager.Stage = 1;
        SceneManager.LoadScene("Game");
    }
    public void ThreeStage()
    {
        GridManager.Stage = 2;
        SceneManager.LoadScene("Game");
    }
}
