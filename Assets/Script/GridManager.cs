using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public delegate void CallBack();

public class GridManager : MonoBehaviour
{
    public static int Stage;
    public GameObject BackImage;
    public int GridDemension; //�� ũ��
    public GameObject[,] Grid; //������ Grid(ĵ��) ��ġ

    public GameObject CandyPrefab;//����� ĵ�� ������
    public Transform CandyParent;//ĵ�� ������Ʈ ���� ���

    public GameObject DoneEffectPrefab;

    private Vector3 posOffset = new Vector3((0.5f + Stage), (0.5f + Stage), 0); //ĵ�� ������Ʈ�� ��ġ

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
        Grid = new GameObject[GridDemension, GridDemension]; //�� ũ�� x, y ���� ũ�⸦ ����
        InitGrid();

    }
/*    public Vector2Int PTGridIndex(Vector3 pos) //Grid��ġ 0.5����
    {
        return new Vector2Int((int)(pos.x - posOffset.x), (int)(pos.y - posOffset.y));
    }*/
    public Vector3 GridIntexTP(int row, int col)
    {
        //������Ʈ�� ��ġ�� ���� Gird(col, row)�� ��ġ���� posOffset ���� �߰��Ͽ� ������Ʈ ��ġ
        return new Vector3(col + posOffset.x, row + posOffset.y, 0);
    }


    void InitGrid()
    {
        //������� Grid������ŭ �ݺ� ����
        for (int col = 0; col < GridDemension; col++)
        {
            for (int row = 0; row < GridDemension; row++)
            {
                //ĵ�� ������Ʈ�� ����, �ش� ��ġ�� + posOffset��ŭ �̵� ��Ų �� ��ġ
                var candy = Instantiate(CandyPrefab, new Vector3(col, row, 0) + posOffset , Quaternion.identity);
                candy.transform.SetParent(CandyParent); //CandyParent�� �ڽ����� �ֱ�
                Candy c = candy.GetComponent<Candy>();
                //ĵ���� ���� ��ġ�� �޾ƿ��� �G Grid �迭�� �ش� ĵ�� ��ġ �ֱ�
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
        //HashSet : List�� ����ϳ� �ߺ��� ������� �ʰ� ������ �������� �ʾ� �����͸� ������ �� ���, ã�� �ӵ��� �ξ� ����
        var match = new HashSet<GameObject>();
        for (int row = 0; row < GridDemension; row++)
        {
            for (int col = 0; col < GridDemension; col++)
            {
                //grid�� ���� Grid �迭 ����
                var grid = Grid[row, col];
                //Index�� grid������Ʈ�� �ִ� spriteIndex ����
                var Index = grid.GetComponent<Candy>().spriteIndex;
                //Hor�� ���� row, col, Index���� ���� ã�Ƴ� matched List�� ����
                var Hor = GetHorizontalMatch(row, col, Index);
                if (Hor.Count >= 2)//���� Hor�� ������ 2 �̻��̸�(col ���� ���� 3)
                {
                    //match�� Hor�� ��� ��Ҹ� �����ϵ��� ����
                    match.UnionWith(Hor);
                    match.Add(grid); // ���տ� ��Ҹ� �߰��ϰ� ���������� �߰� �Ǿ����� ���θ� ��Ÿ���� ��ȯ��
                }
                //Ver�� ���� Verrow, col, Index���� ���� ã�Ƴ� matched List�� ����
                var Ver = GetVerticalMatch(row, col, Index);
                if (Ver.Count >= 2) // Ver ������ 2�̻��̸�
                {
                    //match�� Ver�� ��� ��Ҹ� �����ϵ��� �����ϸ� �߰� �Ǿ����� ���θ� ��Ÿ���� ��ȯ��(Add)
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
        //�ش� col ���� +1�� �߰��ϰ� Grid������ŭ �ݺ�����
        for (int i = col + 1; i < GridDemension; i++)
        {
            //���� �ش� ���� i�� spriteIndex(�̹���)�� ���� ���õ� spIndex�� ���ٸ�
            if (Grid[row, i].GetComponent<Candy>().spriteIndex == spIndex)
            {
                //matched�� �ش��ϴ� Grid ������Ʈ �迭�� �߰�
                matched.Add(Grid[row, i]);
            }
            else //���� ���� �ʴٸ�
                break; //�ݺ� ����
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
    //�ܺο��� �����ϸ� ������Ʈ�� �ı��� �� �ڷ�ƾ�� ���߱⿡ �Ŵ������� ����
    public void RunCheckAndRemoveAndFill()
    {
        StartCoroutine(CheckAndRemoveandFill());
    }
    //��Ī Ȯ�� - ���� - ������ ������Ʈ ������ - ��Ī Ȯ��
    IEnumerator CheckAndRemoveandFill()
    {
        while (true)
        {
            //match�� HashSet �� ����
            var match = CheckAllBoardMatch();
            if (match.Count == 0) //match�� ������ 0�̸�
                break; //�ݺ��� Ż��
            DestroyCandy(match);//match�� HashSet ������Ʈ�� DestroyCandy ����
            FillBlank(); //������ ������Ʈ ������
            //���� allMovDone�� true�� ����
            yield return new WaitUntil(() => allMoveDone == true);
        }
    }
   //3�� �̻��� ������Ʈ ���� 
    public void DestroyCandy(HashSet<GameObject> Dcandy)
    {
        //count�� Dcandy���� ����
        var count = Dcandy.Count;
        foreach (var go in Dcandy)
        {
            //DoneEffectPrefab(eff)�� Dcandy�� ��ġ�� ����
            var eff = Instantiate(DoneEffectPrefab, go.transform.position, Quaternion.identity);
            //������ ������Ʈ�� sprite(�̹���)�� �ش� ��ġ�� �̹����� ����
            eff.GetComponent<DoneE>().spr = go.GetComponent<SpriteRenderer>().sprite;
            //�ش� ��ġ�� Grid�� �����ϸ� �ش� ��ġ ������Ʈ ����
;            Grid[go.GetComponent<Candy>().row, go.GetComponent<Candy>().column] = null;
            Destroy(go);

        }
        Paudio.Play();
        Score += count * 10;
        scoretxt.text = "Score : " + Score.ToString();
        GameOverTxt.text = Score.ToString();
    }

    //��ĭ ä���
    public void FillBlank()
    {
        
        allMoveDone = false;
        for(int col = 0; col < GridDemension; col++)
        {
            //candies�� Getcol�� ��⿭ ����
            var candies = Getcol(col);
            //NewCandy ��⿭�� col ���� ���� �迭ũ�⿡ candies�� ������ŭ �� ���� �޾ƿ���( 
            var newCandy = NewCandy(col, GridDemension - candies.Count); 
            foreach(var item in newCandy)
            {
                //candies ��⿭�� newCandy ��⿭ �߰�
                candies.Enqueue(item);
            }
            //Grid �迭 ũ�⸸ŭ �ݺ�
            for(int row = 0; row < GridDemension; row++)
            {
                //cnd�� candies�� �տ������� ���� ������ ���
                var cnd = candies.Dequeue();
                //candy�� ��⿭���� ���� ������Ʈ�� ������Ʈ �ҷ�����
                var candy = cnd.GetComponent<Candy>();
                //candy�� row ���� ���� row�� �ƴϸ�
                if(candy.row != row)
                {
                    //�ش� ������Ʈ�� MoveToBlank����
                    candy.MoveToBlank(row, col, FillBlankMoveCB);
                }
            }
        }
    }

    Queue<GameObject> Getcol(int col)
    {
        //Queue : ��⿭ - �����ͳ� �۾��� �Է��� ������� ó��
        Queue<GameObject> obj = new Queue<GameObject>();
        for (int row = 0; row < GridDemension; row++)
        {
            //Grid�� ��ġ(row)�� null�� �ƴϸ�
            if(Grid[row, col] != null)
            {
                //��⿭�� �ش� Grid �迭 �߰�
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
            //candyprefab ���� �� ���� col(x) ��ġ�� �� ��(y)�� ����(GridDemension + i)
            var candy = Instantiate(CandyPrefab, new Vector3(col, GridDemension + i, 0) + posOffset, Quaternion.identity);
            //������ ������Ʈ�� �ڽ����� �ֱ�
            candy.transform.SetParent(CandyParent);
            //�ش� ��⿭�� �߰�
            res.Enqueue(candy);
        }
        return res;
    }

    void FillBlankMoveCB()
    {
        //allMoveDone�� isallMoveDone�� ���� true false �� ����
        allMoveDone = isallMoveDone();
    }
    bool isallMoveDone()
    {
        for (int row = 0; row < GridDemension; row++)
        {
            for (int col = 0; col < GridDemension; col++)
            {
                //���� Grid �ȿ� �ִ� Candy��ũ��Ʈ�� MoveDone�� false�� �� false
                if (Grid[row, col].GetComponent<Candy>().moveDone == false)
                    return false;
            }
        }
        return true; //�ƴ� �� true
    }

}
