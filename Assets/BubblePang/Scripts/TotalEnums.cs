using System;
using System.Collections.Generic;
using UnityEngine;
//각종 enum 모음.
public class TotalEnums
{

}

//컨텐츠 난이도
public enum Difficulty : int
{
    Easy,
    Normal,
}
// 컨텐츠 테마
public enum Theme: int
{
    Dino, Animal, Car, Ocean
}

// 오브젝트 경로 설정
public enum Where: int
{
    Ground, Sea, Fly, back
}

//컨텐츠에서 활용될 컨텐츠 파라미터
public class ContentsParameter: EventArgs
{
    //컨텐츠 테마 지정
    public Theme theme;
    //컨텐츠 타입(글자) 지정
    public BubblePang.ContentsType contents;
    //컨텐츠 난이도 지정
    public Difficulty difficult;
    //사람 숫자
    public int person;
    //랜덤 지정
    public int[] shufflepart;
}
//각 플레이어 UI에서 활용될 파라미터
public class PlayerParameter : EventArgs
{
    //몇번째 버튼?, 눌러야 할 갯수
    public int buttonindex, buttonCount, index, indexmax;

    //플레이어별로 버튼은 랜덤으로 섞어야 함....
    public int[] shuffleindex;
}
