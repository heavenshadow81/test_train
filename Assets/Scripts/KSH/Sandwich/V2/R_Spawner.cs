using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_Spawner : MonoBehaviour
{
    private SandKind kind;

    public List<Sand> LaiseSandList = new List<Sand>();

    public int sandCnt = 0;

    private void OnEnable() =>  ActionProcess.SandDeletes += SandDelateAction;

    private void OnDisable() => ActionProcess.SandDeletes -= SandDelateAction;


    // 샌드위치 재료 세팅
    public async UniTask SandSetting(SandKind kind)
    {
        ++sandCnt;
        var sand = SandWichManager.instance.sandPool.Get();     //생성
        sand.transform.position = transform.position;                 //위치 
        sand.SnadKind = kind;                                            //부속 상태
        await  sand.SpriteLoad(sand.SnadKind);                       //스프라이트 로드
        sand.sandRnder.sortingOrder = sandCnt;                      //스프라이트 솔팅
        //쌓기 
        sand.Raise((sand.sandRnder.sortingOrder - 1) * 0.5f, 0.3f, SandWichManager.instance.curve);
        //리스트에 저장
        LaiseSandList.Add(sand);
    }

    //Action 참조용
    public void SandDelateAction()
    {
        SandDelete().Forget();
    }

    /// <summary>
    /// 샌드위치 밑으로 내리고 비우기
    /// </summary>
    /// <returns></returns>
    public async UniTask SandDelete()
    {
        //쌓인 샌드 리스트
        foreach (var item in LaiseSandList)
        {
            //샌드위치 부속들 밑으로 이동
            item.transform.DOMoveY(-7f, 0.5f);
            await UniTask.Delay(TimeSpan.FromSeconds(SandWichManager.instance.gameSpeed));
        }
        //전부 밑으로 이동하면
        foreach (var item in LaiseSandList)
        {
            //비활성화 (풀로 집어넣음)
            item.Release();
        }
        
        await UniTask.Delay(TimeSpan.FromSeconds(SandWichManager.instance.gameSpeed), DelayType.UnscaledDeltaTime, cancellationToken: SandWichManager.instance._source.Token);
        //리스트 클리어
        LaiseSandList.Clear();
        sandCnt = 0;
      //  SandWichManager.instance.IsGame = true;
    }
}
