using System.Collections;
using System.Collections.Generic;

using UnityEngine;


/// <summary>
/// Json Data : 서버 데이터 추가 예정...!
/// </summary>
//콘텐츠 종류
enum Contents
{
    PuzzleAnimal, PuzzleOcean, PuzzleInsect, PuzzleDino, ShadowAnimal, ShadowOcean, ShadowInsect, ShadowDino, OrderingNumberDino, OrderingKoreanDino, OrderingEnglishDino, OrderingNuberAnimal, OrderingKoreanAnimal, OrderingEnglishAnimal, MazeAnimal
}
//플레이어 정보
class PlayerInfo
{
    public string uid;
    //uid를 기반으로 서버 data table의 콘텐츠 테이블을 검색하여...> 해당 콘텐츠 실행 기록을 검색..

    public IDictionary<Contents, ContentsResult> result = new Dictionary<Contents, ContentsResult>();
    //생성자..
    public PlayerInfo(string uid)
    {
        this.uid = uid;
        for (int i = 0; i < (int)Contents.MazeAnimal + 1; i++)
        {
            result.Add((Contents)i, new ContentsResult());
        }
    }

}
//콘텐츠별로 저장할 정보...!!!
class ContentsResult
{
    //콘텐츠 결과
    public IDictionary<System.DateTime, int> contentsMaxPoint = new Dictionary<System.DateTime, int>();
    //콘텐츠 반복 몇 번
    public int contentsIterated = 0;
}

//콘텐츠 저장용
public class ContentsCollections : MonoBehaviour
{
    [SerializeField]
    Contents contents;

    List<PlayerInfo> playerinfo = new List<PlayerInfo>();
    private void OnEnable()
    {
        Coding.ContentsController.Instance.Final += Final;
    }

    private void OnDisable()
    {
        Coding.ContentsController.Instance.Final -= Final;
    }
    //결과 
    void Final()
    {
        foreach(var player in playerinfo)
        {
            player.result[contents].contentsIterated++;

            //날짜별로 점수를 저장하기...
            foreach (var days in player.result[contents].contentsMaxPoint.Keys)
            {
                if (days.Day == System.DateTime.Now.Day && days.Year == System.DateTime.Now.Year && days.Month == System.DateTime.Now.Month)
                {
                    player.result[contents].contentsMaxPoint[days] = 10;
                }
                else
                {
                    player.result[contents].contentsMaxPoint.Add(System.DateTime.Now, 10);
                }
            }
        }
        
    }
    //카드를 찍은 경우 : 카드효과 추가!!!
    void CheckCard(string uid)
    {
        foreach(var player in playerinfo)
        {
            if(player.uid == uid)
            {
                return;
            }
        }
        playerinfo.Add(new PlayerInfo(uid));
    }
}
