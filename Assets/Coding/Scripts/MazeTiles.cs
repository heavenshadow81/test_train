using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//미로 타일 리소스 들고 있는 스크립트...
public class MazeTiles : MonoBehaviour
{
    #region 변수
    static MazeTiles _instance;
    //읽기 전용 싱글턴 조건
    public static MazeTiles Instance
    {
        get => _instance;
    }
    //맵 타일 종류
    [SerializeField]
    GameObject[] LeftTopTile, RightTopTile, LeftBottomTile, RightBottomTile, HorizontalTile, VerticalTile, notUpTile, notLeftTile, notRightTile, notDownTile, UpTile, DownTile, LeftTile, RightTile, OuterTile;
    //입힐 옷, 폭탄
    [SerializeField]
    GameObject Costumes, Bombs, Start, End;
    [SerializeField]
    GameObject[] easyMap, normalMap;
    [SerializeField]
    Transform[] easyStart, normalStart;
    public GameObject[] EasyMap { get => easyMap; }
    public GameObject[] NormalMap { get => normalMap; }
    public Transform[] EasyStart { get => easyStart; }
    public Transform[] NormalStart { get => normalStart; }
    //몬스터
    [SerializeField]
    GameObject[] monsters;
    public GameObject[] Monsters { get => monsters; }
    //변경된 타일
    [SerializeField]
    Material mat;
    public Material Mat { get => mat; }
    #endregion
    #region 유니티 함수
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }

    #endregion
    #region 함수
    //읽기 전용으로 받기 위해....
    public GameObject[] Tile(TileType type)
    {
        GameObject[] tile = type switch
        {
            TileType.LeftTop => LeftTopTile,
            TileType.RightTop => RightTopTile,
            TileType.LeftBottom => LeftBottomTile,
            TileType.RightBottom => RightBottomTile,
            TileType.Horizontal => HorizontalTile,
            TileType.Vertical => VerticalTile,
            TileType.notUp => notUpTile,
            TileType.notLeft => notLeftTile,
            TileType.notRight => notRightTile,
            TileType.notDown => notDownTile,
            TileType.Up => UpTile,
            TileType.Left => LeftTile,
            TileType.Right => RightTile,
            TileType.Down => DownTile,
            TileType.Outer => OuterTile,
            _ => null,
        };
        return tile;

    }
    //각 타일 종류 갯수 반환
    public int TileLength(TileType type)
    {
        int len = type switch
        {
            TileType.LeftTop => LeftTopTile.Length,
            TileType.RightTop => RightTopTile.Length,
            TileType.LeftBottom => LeftBottomTile.Length,
            TileType.RightBottom => RightBottomTile.Length,
            TileType.Horizontal => HorizontalTile.Length,
            TileType.Vertical => VerticalTile.Length,
            TileType.notUp => notUpTile.Length,
            TileType.notLeft => notLeftTile.Length,
            TileType.notRight => notRightTile.Length,
            TileType.notDown => notDownTile.Length,
            TileType.Up => UpTile.Length,
            TileType.Left => LeftTile.Length,
            TileType.Right => RightTile.Length,
            TileType.Down => DownTile.Length,
            TileType.Outer => OuterTile.Length,
            _ => 0,
        };
        return len;
        
    }
    //아이템 종류
    public GameObject Item(ItemType type)
    {
        GameObject item = type switch
        {
            ItemType.Costume => Costumes,
            ItemType.Bomb => Bombs,
            ItemType.Start => Start,
            ItemType.End => End,
            _ => null,
        };
        return item;
        
    }
    
    #endregion
}
