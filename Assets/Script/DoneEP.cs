using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

public class DoneEP : MonoBehaviour
{
    public Sprite sp;

    float fadeSpeed; //DoneE ��ũ��Ʈ�� ���ǵ� �ҷ�����
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
    float A = 1.0f; // ���� ��
    IEnumerator DoFade()
    {
        while (true) // �ݺ� ����
        {
            A -= fadeSpeed; //���� �� ����
            //������Ʈ�� ������ ���� �پ�� �� �ֵ���
            sr.material.color = new Color(sr.material.color.r, sr.material.color.g, sr.material.color.b, A);
            if (A <= 0) break; //������Ʈ ������ 0���� �۰ų� ������ �ݺ� ����
            yield return null;
        }
        Destroy(gameObject); //������Ʈ ����
        if(transform.parent != null) //������Ʈ�� �ڽ����� �� �ִٸ�
        {
            Destroy(transform.parent.gameObject); //�ڽ� ����
        }
    }

}
