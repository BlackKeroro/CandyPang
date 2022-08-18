using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class Candy : MonoBehaviour
{
    public List<Sprite> sprite;
    public int spriteIndex;

    private SpriteRenderer render;
    private static Candy select;

    public float movespeed = 0.35f;

    public int row = -1;
    public int column = -1;
    public bool moveDone = true;

    // Start is called before the first frame update
    void Start()
    {
        //sprite �̹��� ���� ����
        render = GetComponent<SpriteRenderer>();
        spriteIndex = Random.Range(0, sprite.Count);
        render.sprite = sprite[spriteIndex];
    }

    private void OnMouseDown()
    {
        if (select == this) //�ڽ��� ���ý�
        {
            //���õ� ���¸� �����ϰ� �̹��� ����ȭ�� ��ȯ
            select = null;
            Unselect(); 
            return;
        }
        if (select != null) //�ٸ� ������Ʈ�� ���ý�
        {
            select.Unselect();//���� ���õ� ������Ʈ �̹��� ����ȭ
            if (Vector3.Distance(select.transform.position, transform.position) == 1) //���� �̹� ���õ� ������Ʈ�� ������ ������Ʈ�� �Ÿ��� ���̰� 1�ϋ�
            {
                SwapCandyCheck(select, this, false); //SwapCandyCheck�� �̹� ���õ� ������Ʈ, 2��° ���� ������Ʈ�� ����
                select = null; //���� ���¸� Ǯ�� ��ȯ
                return;
            }
        }
        select = this; //�ٸ� ������ �Ǿ� ���� �ʾ��� ��� �ش� ������Ʈ ����(select)
        Inselect();//�ش� ������Ʈ �̹����� ����

    }
    public void Unselect() //������ �����Ǿ��� ��
    {
        render.color = Color.white;
    }
    public void Inselect() //���� �Ǿ��� ��
    {
        render.color = Color.gray;
    }

    Candy C_a;
    Candy C_b;
    void SwapCandyCheck(Candy a, Candy b, bool Return)
    {
        //C ������Ʈ�� �ҷ��� a, b ������Ʈ ����
        C_a = a; 
        C_b = b;

        //DOTween�� ����� �ִϸ��̼�
        //Append : �ִϸ��̼� ���� / Join : �ִϸ��̼� ����� ���ÿ� ���� / OnComplete : �ݹ� �Լ�
        //������Ʈ a �� ��ġ�� b ��ġ��, ������Ʈ b�� ��ġ�� a�� ��ġ�� �̵�
        //�ݹ� �Լ��� ���� return���� true�� null, false�� swapMoveCom ����
        Sequence seq = DOTween.Sequence();
        seq.Append(a.transform.DOMove(b.transform.position, movespeed))
           .Join(b.transform.DOMove(a.transform.position, movespeed))
           .OnComplete(Return ? (TweenCallback)null : SwapMoveCom);

        //b������Ʈ ��ġ ������ ���� a ������Ʈ�� ��ġ�� ����
        int t_row = a.row;
        int t_col = a.column;
        //a������Ʈ�� Grid[row, column]���� b�� ���� b������Ʈ���� �̹� a�� ���� ����Ǿ� �̸� a���� �����ߴ� t�� ����
        a.GetComponent<Candy>().SetRowColumn(b.row, b.column);
        b.GetComponent<Candy>().SetRowColumn(t_row, t_col);
        //����� a�� b ������Ʈ�� ��ġ ���� GridManager���� ����
        GridManager.I.Grid[a.row, a.column] = a.gameObject;
        GridManager.I.Grid[b.row, b.column] = b.gameObject;
    }

    void SwapMoveCom()
    {
        var match = GridManager.I.CheckAllBoardMatch();
        if(match.Count == 0) //match�� ���ڰ� 0�� ���
        {
            SwapCandyCheck(C_a, C_b, true);//���� ������Ʈ�� ��ġ ���� �����Ͽ� �ٽ� �ǵ�����
        }
        else
        {
            GridManager.I.RunCheckAndRemoveAndFill();
        }
    }

    public void SetRowColumn(int row, int col)
    {
        this.row = row;
        this.column = col;
    }

    CallBack FillMoveCB;
    //
    public void MoveToBlank(int row, int col, CallBack cb)
    {
        FillMoveCB = cb;
        moveDone = false;
        //������Ʈ�� �ش� ��ġ�� �̵�
        transform.DOMove(GridManager.I.GridIntexTP(row, col), movespeed).OnComplete(MoveToBlankDone);

        this.row = row;
        this.column = col;
        //����Grid�� row ���� column�� ���� ���� ������Ʈ�� ���� ��
        if (GridManager.I.Grid[row, column] == this.gameObject)
        {
            //Grid�� ���� ��ġ�� ���� ��Ű��
            GridManager.I.Grid[row, column] = null;
        }
        //Grid���� ��ġ�� ���� ���ӿ�����Ʈ�� ����
        GridManager.I.Grid[row, column] = this.gameObject;


    }
    
    void MoveToBlankDone()
    {
        moveDone = true;
        FillMoveCB?.Invoke();
    }

}
