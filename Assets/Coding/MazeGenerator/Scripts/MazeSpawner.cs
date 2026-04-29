using UnityEngine;
using System.Collections.Generic;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>
/// <summary>
/// 미로 생성 스크립트...>무료 애셋에서 받은 것 중에 일부를 변경하여 사용 중
/// </summary>
public class MazeSpawner : MonoBehaviour
{

    //미로 생성 알고리즘 >> : 생성 방식에 따라 한 쪽에서 갈 수 없는 곳이 생성되기도 함...> 기본으로 둘 것
    public enum MazeGenerationAlgorithm
    {
        PureRecursive,
        RecursiveTree,
        RandomTree,
        OldestTree,
        RecursiveDivision,
    }
    #region 변수
    [SerializeField]
    MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
    //완전 랜덤
    [SerializeField]
    [Tooltip("완전히 랜덤으로 두려면 참으로 특정 랜덤 값으로 두려면 거짓으로 둘 것")]
    bool FullRandom = false;
    //랜덤 생성 값
    [SerializeField]
    [Tooltip("FullRandom이 참이 아닐 경우에만 필요한 숫자(랜덤 값 생성 인자)")]
    int RandomSeed = 12345;

    [SerializeField]
    [Tooltip("기본 바닥")]
    GameObject Floor = null;
    [SerializeField]
    [Tooltip("기본 벽")]
    GameObject Wall = null;
    [SerializeField]
    [Tooltip("기본 기둥")]
    GameObject Pillar = null;
    [SerializeField]
    int Rows = 4;
    [SerializeField]
    int Columns = 5;
    [SerializeField]
    [Tooltip("셀의 너비를 지정합니다")]
    float CellWidth = 5;
    [SerializeField]
    [Tooltip("셀의 높이를 지정합니다")]
    float CellHeight = 5;
    //셀과 셀 사이에 간격을 둘 것인지 결정
    public bool AddGaps = true;
    //Goal에 해당하면 아래의 오브젝트가 생성됨
    public GameObject GoalPrefab = null;

    private BasicMazeGenerator mMazeGenerator = null;
    //미로 생성하게 될 곳
    [SerializeField]
    [Tooltip("미로가 해당 오브젝트 아래에 생성됩니다")]
    Transform Maze, ConceptMap;
    //기존에 이미 만들어진 미로가 있다면 이곳에 모두 더해서 파괴하고 비우기>>미로 오브젝트 생성시 여기에 더해짐
    List<GameObject> preMaze = new List<GameObject>();
    //플레이어 캐릭터
    [SerializeField]
    GameObject player1, player2;
    #endregion
    #region 유니티 함수
    void Start()
    {
        //콘텐츠 시작 이벤트에 함수 더하기
        Coding.ContentsController.Instance.Initialize += (contentsParameter) =>
        {
            if (preMaze.Count > 0)
            {
                foreach (var pre in preMaze)
                {
                    Destroy(pre);
                }
            }
            preMaze.Clear();
            MakeMaze(Maze);
            //MazeOuter();

            SetBomb();
        };
        //콘텐츠 난이도 바꾸는 이벤트에 함수 더하기>미로 가로, 세로 길이 지정
        Coding.ContentsController.Instance.SetParameter += (row, column) =>
         {
             Rows = row;
             Columns = column;
         };

        //콘텐츠 난이도 값 얻기
        Coding.ContentsController.Instance.SetDifficult();
        //미로 만들기>시작 화면 미로 만드는 부분
        MakeMaze(ConceptMap);
    }
    #endregion
    #region 기존 함수
    //미로 만드는 함수
    void MakeMaze(Transform Maze)
    {

        if (!FullRandom)
        {
            Random.InitState(RandomSeed);
        }
        switch (Algorithm)
        {
            case MazeGenerationAlgorithm.PureRecursive:
                mMazeGenerator = new RecursiveMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveTree:
                mMazeGenerator = new RecursiveTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RandomTree:
                mMazeGenerator = new RandomTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.OldestTree:
                mMazeGenerator = new OldestTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveDivision:
                mMazeGenerator = new DivisionMazeGenerator(Rows, Columns);
                break;
        }
        mMazeGenerator.GenerateMaze();
        //시작 위치
        Vector3 startPos = new Vector3(-Columns * CellWidth * 0.5f, 0, -Rows * CellHeight * 0.5f);

        // tiles 초기화
        //CharacterSourceContainer.Instance.Tiles();

        //미로 만들고 나서(기본 셀만 만들고 난 뒤에)... 미로 기본 셀을 바탕으로 맵타일 및 벽을 만드는 구간
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                float x = column * (CellWidth + (AddGaps ? 0.1f : 0));
                float z = row * (CellHeight + (AddGaps ? 0.1f : 0));
                MazeCell cell = mMazeGenerator.GetMazeCell(row, column);
                GameObject tmp;
                tmp = Instantiate(Floor, startPos + new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0));
                tmp.transform.parent = Maze;
                #region 내가 추가한 부분: 맵 타일로 지정
                preMaze.Add(tmp);
                //이곳에 맵 타일을 위치하게 하면 될 것 같음..>WallRight, WallFront, WallLeft, WallBack등으로 파악하여.. > 맵 타일 (특정 오브젝트 배열에서 랜덤으로 선택하게....)> 하면 될 것 같음
                GameObject tile;
                if (cell.WallFront)
                {
                    if (cell.WallBack)
                    {
                        if (cell.WallLeft)
                        {
                            tile = MazeTiles.Instance.Tile(TileType.Right)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.Right))];
                        }
                        else if (cell.WallRight)
                        {
                            tile = MazeTiles.Instance.Tile(TileType.Left)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.Left))];
                        }
                        else
                        {
                            tile = MazeTiles.Instance.Tile(TileType.Horizontal)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.Horizontal))];
                        }
                    }
                    else if (cell.WallRight)
                    {
                        if (cell.WallLeft)
                        {
                            tile = MazeTiles.Instance.Tile(TileType.Down)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.Down))];
                        }
                        else
                        {
                            tile = MazeTiles.Instance.Tile(TileType.RightTop)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.RightTop))];
                        }
                    }
                    else if (cell.WallLeft)
                    {
                        tile = MazeTiles.Instance.Tile(TileType.LeftTop)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.LeftTop))];
                    }
                    else
                    {
                        tile = MazeTiles.Instance.Tile(TileType.notUp)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.notUp))];
                    }
                }
                else if (cell.WallBack)
                {
                    if (cell.WallRight)
                    {
                        if (cell.WallLeft)
                        {
                            tile = MazeTiles.Instance.Tile(TileType.Up)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.Up))];
                        }
                        else
                        {
                            tile = MazeTiles.Instance.Tile(TileType.RightBottom)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.RightBottom))];
                        }
                    }
                    else if (cell.WallLeft)
                    {
                        tile = MazeTiles.Instance.Tile(TileType.LeftBottom)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.LeftBottom))];
                    }
                    else
                    {
                        tile = MazeTiles.Instance.Tile(TileType.notDown)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.notDown))];
                    }
                }
                else if (cell.WallRight)
                {
                    if (cell.WallLeft)
                    {
                        tile = MazeTiles.Instance.Tile(TileType.Vertical)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.Vertical))];
                    }
                    else
                    {
                        tile = MazeTiles.Instance.Tile(TileType.notRight)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.notRight))];
                    }
                }
                else
                {
                    tile = MazeTiles.Instance.Tile(TileType.notLeft)[Random.Range(0, MazeTiles.Instance.TileLength(TileType.notLeft))];
                }
                tmp = Instantiate(tile, startPos + new Vector3(x, 0, z), Quaternion.Euler(Vector3.zero));
                tmp.transform.SetParent(Maze);
                preMaze.Add(tmp);
                // row, column  저장하기
                //print("row" + tile.transform.name + "(" + row + " , " + column + ")");

                //// 맵타일 오브젝트 저장
                //CharacterSourceContainer.Instance.tiles[row].tilePrefabs[column] = tmp;
                //// 맵타일 오브젝트 트랜스폼 저장
                //CharacterSourceContainer.Instance.tiles[row].tilePositions[column] = tmp.transform.position;

                #endregion
                if (cell.WallRight)
                {
                    tmp = Instantiate(Wall, startPos + new Vector3(x + CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 90, 0));// right
                    tmp.transform.parent = Maze;
                    preMaze.Add(tmp);
                }
                if (cell.WallFront)
                {
                    tmp = Instantiate(Wall, startPos + new Vector3(x, 0, z + CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 0, 0));// front
                    tmp.transform.parent = Maze;
                    preMaze.Add(tmp);
                }
                if (cell.WallLeft)
                {
                    tmp = Instantiate(Wall, startPos + new Vector3(x - CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 270, 0));// left
                    tmp.transform.parent = Maze;
                    preMaze.Add(tmp);
                }
                if (cell.WallBack)
                {
                    tmp = Instantiate(Wall, startPos + new Vector3(x, 0, z - CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 180, 0));// back
                    tmp.transform.parent = Maze;
                    preMaze.Add(tmp);
                }
                if (cell.IsGoal && GoalPrefab != null)
                {
                    tmp = Instantiate(GoalPrefab, startPos + new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0));
                    tmp.transform.parent = Maze;
                    preMaze.Add(tmp);
                }
                #region 내가 추가한 부분: 시작, 끝 지점
                if (row == 0 && column == 0)
                {
                    GameObject starttemperate = Instantiate(MazeTiles.Instance.Item(ItemType.Start), startPos + new Vector3(x, 0, z), Quaternion.Euler(Vector3.zero));
                    starttemperate.transform.SetParent(Maze);
                    preMaze.Add(starttemperate);
                    //플레이어1 이동
                    player1.transform.position = startPos + new Vector3(x, 0.7f, z);
                    //캐릭터 이동하게 하거나...
                }
                else if (row == 0 && column == Columns - 1)
                {
                    GameObject endtemperate = Instantiate(MazeTiles.Instance.Item(ItemType.End), startPos + new Vector3(x, 0, z), Quaternion.Euler(Vector3.zero));
                    endtemperate.transform.SetParent(Maze);
                    preMaze.Add(endtemperate);
                    //플레이어 2 이동
                    player2.transform.position = startPos + new Vector3(x, 0.7f, z);
                    //캐릭터 이동하게 하거나...
                }
                #endregion
            }
        }
        if (Pillar != null)
        {
            for (int row = 0; row < Rows + 1; row++)
            {
                for (int column = 0; column < Columns + 1; column++)
                {
                    float x = column * (CellWidth + (AddGaps ? .2f : 0));
                    float z = row * (CellHeight + (AddGaps ? .2f : 0));
                    GameObject tmp = Instantiate(Pillar, startPos + new Vector3(x - CellWidth / 2, 0, z - CellHeight / 2), Quaternion.identity);
                    tmp.transform.parent = Maze;
                    preMaze.Add(tmp);
                    //CharacterSourceContainer.Instance.Tiles();
                }
            }

        }

    }
    #endregion
    #region 내가 추가한 함수
    //미로 외곽
    void MazeOuter()
    {
        //시작 위치
        Vector3 startPos = new Vector3(-Columns * CellWidth * 0.5f, 0, -Rows * CellHeight * 0.5f);
        //기존의 미로가 생성된 곳을 제외하고 바깥의 부분에 랜덤한 타일을 생성
        for (int row = -2; row < Rows + 2; row++)
        {
            for (int column = -2; column < Columns + 2; column++)
            {
                if (row < 0 || column < 0 || row >= Rows || column >= Columns)
                {
                    float x = column * (CellWidth + (AddGaps ? 0.2f : 0));
                    float z = row * (CellHeight + (AddGaps ? 0.2f : 0));
                    GameObject temp;
                    temp = Instantiate(MazeTiles.Instance.Tile(TileType.Outer)[Random.Range(0, MazeTiles.Instance.Tile(TileType.Outer).Length)], startPos + new Vector3(x, 0, z), Quaternion.Euler(Vector3.zero));
                    temp.transform.SetParent(Maze);
                    preMaze.Add(temp);
                }
            }
        }
    }

    //폭탄 추가하기!!!
    void SetBomb()
    {
        List<int> randx = new List<int>();
        List<int> randy = new List<int>();


        int numbers = ContentsOptions.GetDifficult() switch
        {
            Difficult.Easy => 2,
            Difficult.Normal => 4,
            _ => 2,
        };

        for (int i = 1; i < Columns; i++)
        {
            randx.Add(i);
        }
        for (int i = 1; i < Rows; i++)
        {
            randy.Add(i);
        }

        int[] tempx = ShuffleArray(randx.ToArray(), Random.Range(0, 30));
        int[] tempy = ShuffleArray(randy.ToArray(), Random.Range(0, 300));

        randx.Clear();
        randy.Clear();
        for (int i = 0; i < numbers + 2; i++)
        {
            randx.Add(tempx[i]);
            randy.Add(tempy[Random.Range(0, tempy.Length)]);
            print($"폭탄 x위치:{randx[i]}, 폭탄 y위치:{randy[i]}");
        }

        //시작 위치
        Vector3 startPos = new Vector3(-Columns * CellWidth * 0.5f, 0, -Rows * CellHeight * 0.5f);
        //폭탄 겹치지 않게...!
        for (int i = 0; i < numbers; i++)
        {
            var bomb = Instantiate(MazeTiles.Instance.Item(ItemType.Bomb), startPos + new Vector3(randx[i] * CellWidth, 0.7f, randy[i] * CellHeight), Quaternion.Euler(Vector3.zero));
            preMaze.Add(bomb);
            bomb.transform.SetParent(Maze);
        }
        for (int i = 0; i < 2; i++)
        {
            var costume = Instantiate(MazeTiles.Instance.Item(ItemType.Costume), startPos + new Vector3(randx[i + numbers] * CellWidth, 0.7f, randy[i + numbers] * CellHeight), Quaternion.Euler(Vector3.zero));
            preMaze.Add(costume);
            costume.transform.SetParent(Maze);
        }
    }

    //배열 랜덤으로 섞기
    public T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random pran = new System.Random(seed);
        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = pran.Next(i, array.Length);
            T tempitem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempitem;
        }
        return array;
    }
    #endregion
}
