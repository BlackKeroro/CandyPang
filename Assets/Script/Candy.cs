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
        //sprite 이미지 랜덤 적용
        render = GetComponent<SpriteRenderer>();
        spriteIndex = Random.Range(0, sprite.Count);
        render.sprite = sprite[spriteIndex];
    }

    private void OnMouseDown()
    {
        if (select == this) //자신을 선택시
        {
            //선택된 상태를 해제하고 이미지 정상화를 반환
            select = null;
            Unselect(); 
            return;
        }
        if (select != null) //다른 오브젝트를 선택시
        {
            select.Unselect();//기존 선택된 오브젝트 이미지 정상화
            if (Vector3.Distance(select.transform.position, transform.position) == 1) //만약 이미 선택된 오브젝트와 선택한 오브젝트의 거리의 차이가 1일떄
            {
                SwapCandyCheck(select, this, false); //SwapCandyCheck에 이미 선택된 오브젝트, 2번째 선택 오브젝트를 대입
                select = null; //선택 상태를 풀고 반환
                return;
            }
        }
        select = this; //다른 선택이 되어 있지 않았을 경우 해당 오브젝트 선택(select)
        Inselect();//해당 오브젝트 이미지색 변경

    }
    public void Unselect() //선택이 해제되었을 때
    {
        render.color = Color.white;
    }
    public void Inselect() //선택 되었을 때
    {
        render.color = Color.gray;
    }

    Candy C_a;
    Candy C_b;
    void SwapCandyCheck(Candy a, Candy b, bool Return)
    {
        //C 오브젝트에 불러온 a, b 오브젝트 대입
        C_a = a; 
        C_b = b;

        //DOTween을 사용한 애니메이션
        //Append : 애니메이션 실행 / Join : 애니메이션 실행시 동시에 실행 / OnComplete : 콜백 함수
        //오브젝트 a 의 위치를 b 위치에, 오브젝트 b의 위치를 a의 위치로 이동
        //콜백 함수를 통해 return값이 true면 null, false면 swapMoveCom 실행
        Sequence seq = DOTween.Sequence();
        seq.Append(a.transform.DOMove(b.transform.position, movespeed))
           .Join(b.transform.DOMove(a.transform.position, movespeed))
           .OnComplete(Return ? (TweenCallback)null : SwapMoveCom);

        //b오브젝트 위치 변경을 위한 a 오브젝트의 위치를 대입
        int t_row = a.row;
        int t_col = a.column;
        //a오브젝트의 Grid[row, column]값을 b로 대입 b오브젝트에는 이미 a의 값이 변경되어 미리 a값을 대입했던 t를 대입
        a.GetComponent<Candy>().SetRowColumn(b.row, b.column);
        b.GetComponent<Candy>().SetRowColumn(t_row, t_col);
        //변경된 a와 b 오브젝트의 위치 값을 GridManager에서 변경
        GridManager.I.Grid[a.row, a.column] = a.gameObject;
        GridManager.I.Grid[b.row, b.column] = b.gameObject;
    }

    void SwapMoveCom()
    {
        var match = GridManager.I.CheckAllBoardMatch();
        if(match.Count == 0) //match의 숫자가 0일 경우
        {
            SwapCandyCheck(C_a, C_b, true);//기존 오브젝트의 위치 값을 지정하여 다시 되돌리기
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
        //오브젝트를 해당 위치로 이동
        transform.DOMove(GridManager.I.GridIntexTP(row, col), movespeed).OnComplete(MoveToBlankDone);

        this.row = row;
        this.column = col;
        //만약Grid의 row 값과 column이 현재 게임 오브젝트와 같을 때
        if (GridManager.I.Grid[row, column] == this.gameObject)
        {
            //Grid의 현재 위치를 제외 시키기
            GridManager.I.Grid[row, column] = null;
        }
        //Grid의의 위치에 현재 게임오브젝트를 대입
        GridManager.I.Grid[row, column] = this.gameObject;


    }
    
    void MoveToBlankDone()
    {
        moveDone = true;
        FillMoveCB?.Invoke();
    }

}
