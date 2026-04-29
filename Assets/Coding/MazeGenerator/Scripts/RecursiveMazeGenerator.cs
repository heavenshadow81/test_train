using UnityEngine;
using System.Collections;

//<summary>
//Pure recursive maze generation.
//Use carefully for large mazes.
//</summary>
public class RecursiveMazeGenerator : BasicMazeGenerator
{

    public RecursiveMazeGenerator(int rows, int columns) : base(rows, columns)
    {

    }

    public override void GenerateMaze()
    {
        VisitCell(0, 0, Coding.Direction.Start);
    }

    private void VisitCell(int row, int column, Coding.Direction moveMade)
    {
        Coding.Direction[] movesAvailable = new Coding.Direction[4];
        int movesAvailableCount = 0;

        do
        {
            movesAvailableCount = 0;

            //check move right
            if (column + 1 < ColumnCount && !GetMazeCell(row, column + 1).IsVisited)
            {
                movesAvailable[movesAvailableCount] = Coding.Direction.Right;
                movesAvailableCount++;
            }
            else if (!GetMazeCell(row, column).IsVisited && moveMade != Coding.Direction.Left)
            {
                GetMazeCell(row, column).WallRight = true;
            }
            //check move forward
            if (row + 1 < RowCount && !GetMazeCell(row + 1, column).IsVisited)
            {
                movesAvailable[movesAvailableCount] = Coding.Direction.Front;
                movesAvailableCount++;
            }
            else if (!GetMazeCell(row, column).IsVisited && moveMade != Coding.Direction.Back)
            {
                GetMazeCell(row, column).WallFront = true;
            }
            //check move left
            if (column > 0 && column - 1 >= 0 && !GetMazeCell(row, column - 1).IsVisited)
            {
                movesAvailable[movesAvailableCount] = Coding.Direction.Left;
                movesAvailableCount++;
            }
            else if (!GetMazeCell(row, column).IsVisited && moveMade != Coding.Direction.Right)
            {
                GetMazeCell(row, column).WallLeft = true;
            }
            //check move backward
            if (row > 0 && row - 1 >= 0 && !GetMazeCell(row - 1, column).IsVisited)
            {
                movesAvailable[movesAvailableCount] = Coding.Direction.Back;
                movesAvailableCount++;
            }
            else if (!GetMazeCell(row, column).IsVisited && moveMade != Coding.Direction.Front)
            {
                GetMazeCell(row, column).WallBack = true;
            }

            if (movesAvailableCount == 0 && !GetMazeCell(row, column).IsVisited)
            {
                GetMazeCell(row, column).IsGoal = true;
            }

            GetMazeCell(row, column).IsVisited = true;

            if (movesAvailableCount > 0)
            {
                switch (movesAvailable[Random.Range(0, movesAvailableCount)])
                {
                    case Coding.Direction.Start:
                        break;
                    case Coding.Direction.Right:
                        VisitCell(row, column + 1, Coding.Direction.Right);
                        break;
                    case Coding.Direction.Front:
                        VisitCell(row + 1, column, Coding.Direction.Front);
                        break;
                    case Coding.Direction.Left:
                        VisitCell(row, column - 1, Coding.Direction.Left);
                        break;
                    case Coding.Direction.Back:
                        VisitCell(row - 1, column, Coding.Direction.Back);
                        break;
                }
            }

        } while (movesAvailableCount > 0);
    }
}
