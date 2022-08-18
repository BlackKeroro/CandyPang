using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public delegate void CallBack();

public class GridManager : MonoBehaviour
{
    public static int Stage;
    public GameObject BackImage;
    public int GridDemension; //맵 크기
    public GameObject[,] Grid; //생성할 Grid(캔디) 위치

    public GameObject CandyPrefab;//사용할 캔디 프리팹
    public Transform CandyParent;//캔디 오브젝트 넣을 장소

    public GameObject DoneEffectPrefab;

    private Vector3 posOffset = new Vector3((0.5f + Stage), (0.5f + Stage), 0); //캔디 오브젝트의 위치

    public bool allMoveDone = true;

    public AudioSource Paudio;

    public static GridManager I { get; private set; }
    void Awake() 
    { 
        I = this;
    }

    public float Distance = 1.0f;

    public TextMeshProUGUI scoretxt;
    public TextMeshProUGUI GameOverTxt;
    int Score;


    // Start is called before the first frame update
    void Start()
    {

        Paudio = GetComponent<AudioSource>(); 
        if (Stage == 0) GridDemension = 9;
        else if (Stage == 1) GridDemension = 7;
        else if (Stage == 2)
        {
            GridDemension = 6;
            BackImage.transform.position = new Vector3(BackImage.transform.position.x + 0.5f, BackImage.transform.position.y + 0.5f, 0);
        }
        BackImage.transform.localScale = new Vector3(GridDemension, GridDemension, 0);  
        Grid = new GameObject[GridDemension, GridDemension]; //맵 크기 x, y 값의 크기를 지정
        InitGrid();

    }
/*    public Vector2Int PTGridIndex(Vector3 pos) //Grid위치 0.5간격
    {
        return new Vector2Int((int)(pos.x - posOffset.x), (int)(pos.y - posOffset.y));
    }*/
    public Vector3 GridIntexTP(int row, int col)
    {
        //오브젝트의 위치를 현재 Gird(col, row)의 위치에서 posOffset 값을 추가하여 오브젝트 배치
        return new Vector3(col + posOffset.x, row + posOffset.y, 0);
    }


    void InitGrid()
    {
        //만들어진 Grid범위만큼 반복 실행
        for (int col = 0; col < GridDemension; col++)
        {
            for (int row = 0; row < GridDemension; row++)
            {
                //캔디 오브젝트를 생성, 해당 위치에 + posOffset만큼 이동 시킨 후 배치
                var candy = Instantiate(CandyPrefab, new Vector3(col, row, 0) + posOffset , Quaternion.identity);
                candy.transform.SetParent(CandyParent); //CandyParent의 자식으로 넣기
                Candy c = candy.GetComponent<Candy>();
                //캔디의 현재 위치를 받아오기 밎 Grid 배열에 해당 캔디 위치 넣기
                c.SetRowColumn(row, col);
                Grid[row, col] = candy;
            }
        }
        StartCoroutine(WaitAndCheck());
    }
    IEnumerator WaitAndCheck()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(CheckAndRemoveandFill());
    }

    public HashSet<GameObject> CheckAllBoardMatch()
    {
        //HashSet : List와 비슷하나 중복을 허용하지 않고 순서를 보장하지 않아 데이터를 보관할 때 사용, 찾는 속도가 훨씬 빠름
        var match = new HashSet<GameObject>();
        for (int row = 0; row < GridDemension; row++)
        {
            for (int col = 0; col < GridDemension; col++)
            {
                //grid에 현재 Grid 배열 대입
                var grid = Grid[row, col];
                //Index에 grid컴포넌트에 있는 spriteIndex 대입
                var Index = grid.GetComponent<Candy>().spriteIndex;
                //Hor에 현재 row, col, Index값을 통해 찾아낸 matched List값 대입
                var Hor = GetHorizontalMatch(row, col, Index);
                if (Hor.Count >= 2)//만약 Hor의 갯수가 2 이상이면(col 본인 포함 3)
                {
                    //match에 Hor의 모든 요소를 포함하도록 설정
                    match.UnionWith(Hor);
                    match.Add(grid); // 집합에 요소를 추가하고 성공적으로 추가 되었는지 여부를 나타내는 반환값
                }
                //Ver에 현재 Verrow, col, Index값을 통해 찾아낸 matched List값 대입
                var Ver = GetVerticalMatch(row, col, Index);
                if (Ver.Count >= 2) // Ver 갯수가 2이상이면
                {
                    //match에 Ver의 모든 요소를 포함하도록 설정하며 추가 되었는지 여부를 나타내는 반환값(Add)
                    match.UnionWith(Ver);
                    match.Add(grid);
                }
            }
        }
        return match;
    }

    List<GameObject> GetHorizontalMatch(int row, int col, int spIndex)
    {
        List<GameObject> matched = new List<GameObject>();
        //해당 col 값에 +1를 추가하고 Grid범위만큼 반복실행
        for (int i = col + 1; i < GridDemension; i++)
        {
            //만약 해당 행의 i의 spriteIndex(이미지)가 현재 선택된 spIndex와 같다면
            if (Grid[row, i].GetComponent<Candy>().spriteIndex == spIndex)
            {
                //matched에 해당하는 Grid 오브젝트 배열을 추가
                matched.Add(Grid[row, i]);
            }
            else //만약 같지 않다면
                break; //반복 멈춤
        }
        return matched;
    }
    List<GameObject> GetVerticalMatch(int row, int col, int spIndex)
    {
        List<GameObject> matched = new List<GameObject>();
        for (int i = row + 1; i < GridDemension; i++)
        {
            if (Grid[i, col].GetComponent<Candy>().spriteIndex == spIndex)
            {
                matched.Add(Grid[i, col]);
            }
            else
                break;
        }
        return matched;
    }
    //외부에서 실행하면 오브젝트가 파괴될 시 코루틴도 멈추기에 매니저에서 실행
    public void RunCheckAndRemoveAndFill()
    {
        StartCoroutine(CheckAndRemoveandFill());
    }
    //매칭 확인 - 삭제 - 생성된 오브젝트 내리기 - 매칭 확인
    IEnumerator CheckAndRemoveandFill()
    {
        while (true)
        {
            //match에 HashSet 값 대입
            var match = CheckAllBoardMatch();
            if (match.Count == 0) //match의 갯수가 0이면
                break; //반복문 탈출
            DestroyCandy(match);//match의 HashSet 오브젝트에 DestroyCandy 실행
            FillBlank(); //생성된 오브젝트 내리기
            //만약 allMovDone이 true면 실행
            yield return new WaitUntil(() => allMoveDone == true);
        }
    }
   //3개 이상의 오브젝트 제거 
    public void DestroyCandy(HashSet<GameObject> Dcandy)
    {
        //count에 Dcandy갯수 대입
        var count = Dcandy.Count;
        foreach (var go in Dcandy)
        {
            //DoneEffectPrefab(eff)을 Dcandy의 위치에 생성
            var eff = Instantiate(DoneEffectPrefab, go.transform.position, Quaternion.identity);
            //생성된 오브젝트의 sprite(이미지)를 해당 위치의 이미지로 변경
            eff.GetComponent<DoneE>().spr = go.GetComponent<SpriteRenderer>().sprite;
            //해당 위치의 Grid를 제외하며 해당 위치 오브젝트 제거
;            Grid[go.GetComponent<Candy>().row, go.GetComponent<Candy>().column] = null;
            Destroy(go);

        }
        Paudio.Play();
        Score += count * 10;
        scoretxt.text = "Score : " + Score.ToString();
        GameOverTxt.text = Score.ToString();
    }

    //빈칸 채우기
    public void FillBlank()
    {
        
        allMoveDone = false;
        for(int col = 0; col < GridDemension; col++)
        {
            //candies에 Getcol의 대기열 대입
            var candies = Getcol(col);
            //NewCandy 대기열에 col 값과 현재 배열크기에 candies의 갯수만큼 뺀 값을 받아오기( 
            var newCandy = NewCandy(col, GridDemension - candies.Count); 
            foreach(var item in newCandy)
            {
                //candies 대기열에 newCandy 대기열 추가
                candies.Enqueue(item);
            }
            //Grid 배열 크기만큼 반복
            for(int row = 0; row < GridDemension; row++)
            {
                //cnd에 candies를 앞에서부터 값을 꺼내어 사용
                var cnd = candies.Dequeue();
                //candy에 대기열에서 꺼낸 오브젝트의 컴포넌트 불러오기
                var candy = cnd.GetComponent<Candy>();
                //candy의 row 값이 현재 row가 아니면
                if(candy.row != row)
                {
                    //해당 오브젝트의 MoveToBlank실행
                    candy.MoveToBlank(row, col, FillBlankMoveCB);
                }
            }
        }
    }

    Queue<GameObject> Getcol(int col)
    {
        //Queue : 대기열 - 데이터나 작업을 입력한 순서대로 처리
        Queue<GameObject> obj = new Queue<GameObject>();
        for (int row = 0; row < GridDemension; row++)
        {
            //Grid의 위치(row)가 null이 아니면
            if(Grid[row, col] != null)
            {
                //대기열에 해당 Grid 배열 추가
                obj.Enqueue(Grid[row, col]);
            }
        }
        return obj;
    }
    Queue<GameObject> NewCandy(int col, int num)
    {
        Queue<GameObject> res = new Queue<GameObject>();
        for(int i = 0; i < num; i++)
        {
            //candyprefab 생성 및 현재 col(x) 위치의 맨 위(y)에 생성(GridDemension + i)
            var candy = Instantiate(CandyPrefab, new Vector3(col, GridDemension + i, 0) + posOffset, Quaternion.identity);
            //생성된 오브젝트를 자식으로 넣기
            candy.transform.SetParent(CandyParent);
            //해당 대기열에 추가
            res.Enqueue(candy);
        }
        return res;
    }

    void FillBlankMoveCB()
    {
        //allMoveDone에 isallMoveDone을 통한 true false 값 대입
        allMoveDone = isallMoveDone();
    }
    bool isallMoveDone()
    {
        for (int row = 0; row < GridDemension; row++)
        {
            for (int col = 0; col < GridDemension; col++)
            {
                //만약 Grid 안에 있는 Candy스크립트의 MoveDone이 false일 때 false
                if (Grid[row, col].GetComponent<Candy>().moveDone == false)
                    return false;
            }
        }
        return true; //아닐 시 true
    }

}
