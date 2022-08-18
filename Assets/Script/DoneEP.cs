using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

public class DoneEP : MonoBehaviour
{
    public Sprite sp;

    float fadeSpeed; //DoneE 스크립트의 스피드 불러오기
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        //
        fadeSpeed = transform.parent.GetComponent<DoneE>().fadeSpeed;
        sp = transform.parent.GetComponent<DoneE>().spr;
        sr.sprite = sp;

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-60, 60));

        float MoveTime = 0.3f;

        Sequence s = DOTween.Sequence(); //
        var pos = transform.position + (Vector3)Random.insideUnitCircle * Random.Range(0.2f, 0.5f);
        s.Append(transform.DOMove(pos, MoveTime));
        s.Join(transform.DORotate(new Vector3(0, 0, Random.Range(-15f, 15f)), MoveTime))
            .OnComplete(StartFade);

    }

    void StartFade()
    {
        StartCoroutine(DoFade());
    }
    float A = 1.0f; // 투명도 값
    IEnumerator DoFade()
    {
        while (true) // 반복 실행
        {
            A -= fadeSpeed; //투명도 값 감소
            //오브젝트의 투명도가 점점 줄어들 수 있도록
            sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b, A);
            if (A <= 0) break; //오브젝트 투명도가 0보다 작거나 같으면 반복 중지
            yield return null;
        }
        Destroy(gameObject); //오브젝트 삭제
        if(transform.parent != null) //오브젝트가 자식으로 들어가 있다면
        {
            Destroy(transform.parent.gameObject); //자식 삭제
        }
    }

}
