using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//콘텐츠 난이도
public enum Difficult
{
    Easy,
    Normal,
}
//맵 타일 종류
public enum TileType
{
    LeftTop, RightTop, LeftBottom, RightBottom, Horizontal, Vertical, notUp, notLeft, notRight, notDown, Up, Down, Left, Right, Outer
}
//생성할 아이템 종류..
public enum ItemType
{
    Costume, Bomb, Start, End
}
//미로 찾기 알고리즘에 쓰일 enum값...
public enum PathFinderStatus
{
    Not_Initialized, Success, Failure, Running,
}
