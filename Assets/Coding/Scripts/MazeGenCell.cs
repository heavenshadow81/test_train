using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

//맵 생성 스크립트!!!> 콘텐츠 시작 시
/// <summary>
/// 원본 소스코드 : https://github.com/orifmilod/maze-generator/blob/master/Assets/SCRIPTS/Generator.cs
/// 이 생성 스크립트는 임시 보류...: 벽을 직접 생성, 삭제하는 과정에서 맵타일을 지정할 때 다르게 지정되는 버그가 발생하여 이 스크립트는 보류..
/// </summary>

public enum CeType
{
    East,
    West,
    North,
    South,
    NorthEast,
    NorthWest,
    NorthSouth,
    SouthEast,
    SouthWest,
    EastWest,
    DeEast,
    DeWest,
    DeNorth,
    DeSouth,
}


[System.Serializable]
public class Cell
{

    public bool visited;
    public GameObject north;//1
    public GameObject east;//2
    public GameObject west;//3
    public GameObject south;//4
    public CeType celltype;
    public Vector3 pos;
}
public class MazeGenCell : MonoBehaviour
{

    [SerializeField] int RowInput;
    [SerializeField] int ColumnInput;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject[] East, West, South, North, NorthEast, NorthWest, NorthSouth, SouthEast, SouthWest, EastWest, DeEast, DeWest, DeNorth, DeSouth;
    [SerializeField] GameObject start, end;
    private GameObject wallHolder, tempWall;
    private int row, column;
    private float wallLength = 1.0f;
    private Vector3 startPosition;
    private int currentCell = 0;
    public Cell[] cells;
    private int totalCells;
    private int currentNeighbour = 0;
    private int backingUp = 0;
    List<int> cellList;
    List<GameObject> maptiles = new List<GameObject>();


    //임시로 미로 만들기 함수 확인용
    private void Start()
    {
        Coding.ContentsController.Instance.Initialize += (parameter) =>
        {
            GenerateNewMaze();
            StartCoroutine(SetMazeHidden());
        };
        Coding.ContentsController.Instance.SetParameter += (r, c) =>
        {
            row = r;
            column = c;
        };
    }
    //미로 활성화
    public void SetMazeActive()
    {
        wallHolder?.SetActive(true);
    }

    IEnumerator SetMazeHidden()
    {
        yield return new WaitForSeconds(0.05f);
        wallHolder?.SetActive(false);
        yield break;
    }
    public void GenerateNewMaze()
    {
        
        row = RowInput;
        column = ColumnInput;
        totalCells = row * column;
        //Destroying previous maze if exist
        if (wallHolder != null)
        {
            Destroy(wallHolder);
            wallHolder = null;
        }
        foreach(var a in maptiles)
        {
            Destroy(a);
        }
        maptiles.Clear();
        CreateWall();


    }
    /// <summary>
    /// Creating wall gameobject based on rows and columns given
    /// </summary>
    public void CreateWall()
    {
        wallHolder = new GameObject();
        wallHolder.name = "Walls";
        wallHolder.tag = "Walls";

        startPosition = transform.position + new Vector3((-column / 2) + wallLength / 2, 0.0f, (-row / 2) + wallLength / 2);
        Vector3 myPos = startPosition;

        //for creating columns
        for (int a = 0; a < row; a++)
        {
            for (int b = 0; b <= column; b++)
            {
                myPos = new Vector3(startPosition.x + (b * wallLength) - wallLength / 2, 0.0f, startPosition.z + (a * wallLength) - wallLength / 2);
                tempWall = Instantiate(wall, myPos, Quaternion.identity);
                tempWall.name = "column " + a + "," + b;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        //for creating rows
        for (int a = 0; a <= row; a++)
        {
            for (int b = 0; b < column; b++)
            {
                myPos = new Vector3(startPosition.x + (b * wallLength), 0.0f, startPosition.z + (a * wallLength) - wallLength);
                tempWall = Instantiate(wall, myPos, Quaternion.Euler(0, 90, 0));
                tempWall.name = "row " + a + "," + b;
                tempWall.transform.parent = wallHolder.transform;
                #region 추가된 부분 : 시작, 끝 지정
                if (a == 0 && b == 0)
                {
                    GameObject t = Instantiate(start, myPos, Quaternion.identity);
                    t.transform.SetParent(wallHolder.transform);
                }
                else if (a == 0 && b == column - 1)
                {
                    GameObject t = Instantiate(end, myPos, Quaternion.identity);
                    t.transform.SetParent(wallHolder.transform);
                }
                #endregion
            }
        }
        CreateCells();
    }

    /// <summary>
    /// Assigning created walls to the cells direction (north,east,west,south)
    /// </summary>
    public void CreateCells()
    {
        cellList = new List<int>();
        int children = wallHolder.transform.childCount;
        GameObject[] allWalls = new GameObject[children];
        cells = new Cell[totalCells];

        int eastWestProccess = 0;
        int childProcess = 0;
        int termCount = 0;
        int cellProccess = 0;

        //Assigning all the walls to the allwalls array
        for (int i = 0; i < children; i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }

        //Assigning walls to the cells
        for (int j = 0; j < column; j++)
        {
            cells[cellProccess] = new Cell();

            cells[cellProccess].west = allWalls[eastWestProccess];
            cells[cellProccess].south = allWalls[childProcess + (column + 1) * row];
            termCount++;
            childProcess++;
            cells[cellProccess].north = allWalls[(childProcess + (column + 1) * row) + column - 1];

            eastWestProccess++;
            cells[cellProccess].east = allWalls[eastWestProccess];

            cells[cellProccess].pos = cells[cellProccess].north.transform.position;


            cellProccess++;
            if (termCount == column && cellProccess < cells.Length)
            {
                eastWestProccess++;
                termCount = 0;
                j = -1;
            }

        }
        CreateMaze();
    }

    /// <summary>
    /// Getting a random neighbour if not visited and wall between them
    /// </summary>
    void GiveMeNeighbour()
    {
        int length = 0;
        int[] neighbour = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;
        check = (currentCell + 1) / column;
        check -= 1;
        check *= column;
        check += column;
        //north
        if (currentCell + column < totalCells)
        {
            if (cells[currentCell + column].visited == false)
            {
                neighbour[length] = currentCell + column;
                connectingWall[length] = 1;
                length++;
            }
        }
        //east
        if (currentCell + 1 < totalCells && (currentCell + 1) != check)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbour[length] = currentCell + 1;
                connectingWall[length] = 2;
                length++;
            }
        }
        //west
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbour[length] = currentCell - 1;
                connectingWall[length] = 3;
                length++;
            }
        }
        //south
        if (currentCell - column >= 0)
        {
            if (cells[currentCell - column].visited == false)
            {
                neighbour[length] = currentCell - column;
                connectingWall[length] = 4;
                length++;
            }
        }

        //Getting random neighbour and destroying the wall
        if (length != 0)
        {
            int randomNeighbour = Random.Range(0, length);
            currentNeighbour = neighbour[randomNeighbour];
            DestroyWall(connectingWall[randomNeighbour]);
        }

        else if (backingUp > 0)
        {
            currentCell = cellList[backingUp];
            backingUp--;
        }

    }

    void CreateMaze()
    {
        bool startedBuilding = false;
        int visitedCells = 0;
        while (visitedCells < totalCells)
        {
            if (startedBuilding)
            {
                GiveMeNeighbour();

                if (!cells[currentNeighbour].visited && cells[currentCell].visited)
                {
                    int randomNeighbour = Random.Range(0, 5);
                    cells[currentNeighbour].visited = true;
                    visitedCells++;
                    cellList.Add(currentCell);
                    currentCell = currentNeighbour;

                    if (cellList.Count > 0)
                        backingUp = cellList.Count - 1;
                }
            }
            else
            {
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }
        }
        #region 추가된 부분(celltype) : enum값 변경> 맵타일 놓기 지정을 위한 선택
        foreach (var cel in cells)
        {
            if (cel.north != null)
            {
                if (cel.south != null)
                {
                    if (cel.west != null)
                    {
                        cel.celltype = CeType.DeEast;
                    }
                    else if (cel.east != null)
                    {
                        cel.celltype = CeType.DeWest;
                    }
                    else
                    {
                        cel.celltype = CeType.NorthSouth;
                    }
                }
                else if (cel.east != null)
                {
                    if (cel.west != null )
                    {
                        cel.celltype = CeType.DeSouth;
                    }
                    else
                    {
                        cel.celltype = CeType.NorthEast;
                    }
                }
                else if (cel.west != null)
                {
                    cel.celltype = CeType.NorthWest;
                }
                else
                {
                    cel.celltype = CeType.North;
                }
            }
            else if (cel.south != null)
            {
                if (cel.east != null)
                {
                    if (cel.west != null)
                    {
                        cel.celltype = CeType.DeNorth;
                    }
                    else
                    {
                        cel.celltype = CeType.SouthEast;
                    }
                }
                else if (cel.west != null)
                {
                    cel.celltype = CeType.SouthWest;
                }
                else
                {
                    cel.celltype = CeType.South;
                }
            }
            else if (cel.east != null)
            {
                if (cel.west != null)
                {
                    cel.celltype = CeType.EastWest;
                }
                else
                {
                    cel.celltype = CeType.East;
                }
            }
            else
            {
                cel.celltype = CeType.West;
            }
            GameObject currentmaptile = East[0];
            switch (cel.celltype)
            {
                case CeType.East:
                    currentmaptile = East[Random.Range(0, East.Length)];
                    break;
                case CeType.West:
                    currentmaptile = West[Random.Range(0, East.Length)];
                    break;
                case CeType.North:
                    currentmaptile = North[Random.Range(0, East.Length)];
                    break;
                case CeType.South:
                    currentmaptile = South[Random.Range(0, East.Length)];
                    break;
                case CeType.NorthEast:
                    currentmaptile = NorthEast[Random.Range(0, East.Length)];
                    break;
                case CeType.NorthWest:
                    currentmaptile = NorthWest[Random.Range(0, East.Length)];
                    break;
                case CeType.NorthSouth:
                    currentmaptile = NorthSouth[Random.Range(0, East.Length)];
                    break;
                case CeType.SouthEast:
                    currentmaptile = SouthEast[Random.Range(0, East.Length)];
                    break;
                case CeType.SouthWest:
                    currentmaptile = SouthWest[Random.Range(0, East.Length)];
                    break;
                case CeType.EastWest:
                    currentmaptile = EastWest[Random.Range(0, East.Length)];
                    break;
                case CeType.DeEast:
                    currentmaptile = DeEast[Random.Range(0, East.Length)];
                    break;
                case CeType.DeNorth:
                    currentmaptile = DeNorth[Random.Range(0, East.Length)];
                    break;
                case CeType.DeSouth:
                    currentmaptile = DeSouth[Random.Range(0, East.Length)];
                    break;
                case CeType.DeWest:
                    currentmaptile = DeWest[Random.Range(0, East.Length)];
                    break;
            }
            GameObject a = Instantiate(currentmaptile, cel.pos, Quaternion.identity);
            maptiles.Add(a);
            a.transform.SetParent(wallHolder.transform);
            #endregion
        }
    }

    void DestroyWall(int neighbour)
    {
        switch (neighbour)
        {
            //case 1 means north wall
            case 1:

                Destroy(cells[currentCell].north);
                cells[currentCell].north = null;
                break;

            //case 2 means east wall
            case 2:
                Destroy(cells[currentCell].east);
                cells[currentCell].east = null;
                break;

            //case 3 means west wall
            case 3:
                Destroy(cells[currentCell].west);
                cells[currentCell].west = null;
                break;

            //case 4 means south wall
            case 4:
                Destroy(cells[currentCell].south);
                cells[currentCell].south = null;
                break;

            default:
                break;
        }
    }
}

