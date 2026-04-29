using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//콘텐츠 옵션 저장 클래스 : 씬 위에는 없음
public static class ContentsOptions
{
    #region 변수
    //플레이어 숫자
    static int playernumber = 2;
    //난이도
    static Difficult difficult = playernumber == 2 ? Difficult.Normal : Difficult.Easy;

    #endregion
    #region 함수
    //읽기 전용으로 ..
    public static int GetPlayerNumber() => playernumber;
    //읽기 전용으로..
    public static Difficult GetDifficult() => difficult;
    //플레이어 설정
    public static void SetPlayer(int num) 
    {
        playernumber = num;
        difficult = playernumber switch
        {
            2 => Difficult.Normal,
            _ => difficult,
        };
    }
    //난이도 설정
    public static void SetDifficult(int diff)
    {
        difficult = (Difficult)diff;
        
        playernumber = difficult switch
        {
            Difficult.Easy => 1,
            _=> playernumber,
        };
    }
    #endregion

}
