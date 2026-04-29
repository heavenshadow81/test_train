using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_Spawner : MonoBehaviour
{
    //쌓인 샌드위치 리스트
    public List<Sand> LaiseSandList = new List<Sand>();
    //쌓인 갯수
    public int sandCnt = 0;

    private void OnEnable()
    {
        ActionProcess.SandDeletes += SandDelete;
        ActionProcess.SandAllRaise += AllLaise;
    }

    private void OnDisable()
    {
        ActionProcess.SandDeletes -= SandDelete;
        ActionProcess.SandAllRaise -= AllLaise;
    }

    //쌓기 
    public async void AllLaise()
    {
        await UniTask.Yield();
        //게임이 플레이 상태일때
        if (SandWichManager.instance.stateClass.state == GameState.GamePlay)
        {
            //사운드 
            SandWichManager.instance.soundMgr.Raining();
            //빵 타입으로 이미지 생성및 로드
            SandSetting(SandKind.Bread);
            
            await UniTask.Delay(TimeSpan.FromSeconds(SandWichManager.instance.gameSpeed), DelayType.UnscaledDeltaTime, cancellationToken: SandWichManager.instance._source.Token);

            for (int i = 1; i <= 5; i++)
            {
                //빵을 제외한 나머지로 
                //이미지 생성 및 로드
                SandSetting((SandKind)UnityEngine.Random.Range(2, (int)SandKind.MAX));
                await UniTask.Delay(TimeSpan.FromSeconds(SandWichManager.instance.gameSpeed), DelayType.UnscaledDeltaTime, cancellationToken: SandWichManager.instance._source.Token);
            }
            //마지막 빵 이미지 생성 및 로드
            SandSetting(SandKind.Bread);

            await UniTask.Delay(TimeSpan.FromSeconds(SandWichManager.instance.gameSpeed), DelayType.UnscaledDeltaTime, cancellationToken: SandWichManager.instance._source.Token);

            SandWichManager.instance.IsGame = true;
        }
    }

    //샌드위치 부속 하나씩 세팅
    private async void SandSetting(SandKind kind)
    {
        if (SandWichManager.instance.stateClass.state == GameState.GamePlay)
        {
            //샌드 카운트 증가
            ++sandCnt;
            //샌드위치 부속 생성
            var sand = SandWichManager.instance.sandPool.Get();
            //생성위치
            sand.transform.position = transform.position;
            //샌드위치 타입저장
            sand.SnadKind = kind;
            //샌드 이미지 로드
            await sand.SpriteLoad(sand.SnadKind);
            //샌드 솔팅
            sand.sandRnder.sortingOrder = sandCnt;
            await UniTask.Delay(TimeSpan.FromSeconds(SandWichManager.instance.gameSpeed));
            //세팅 완료된 샌드위치 아래로 이동
            sand.Raise((sand.sandRnder.sortingOrder - 1) * 0.5f , 0.3f, SandWichManager.instance.curve);
            //샌드리스트 저장
            LaiseSandList.Add(sand);
        }
    }

    //샌드위치 지우기
    public async void SandDelete()
    {
        //게임중일때
        if (SandWichManager.instance.stateClass.state == GameState.GamePlay)
        {
            //샌드리스트
            foreach (var item in LaiseSandList)
            {
                //아래로 이동시킴
                item.transform.DOMoveY(-7f, 0.5f);
                await UniTask.Delay(TimeSpan.FromSeconds(SandWichManager.instance.gameSpeed));
            }
            //이동 끝난 후 하나씩 제거
            foreach (var item in LaiseSandList)
            {
                item.Release();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(SandWichManager.instance.gameSpeed), DelayType.UnscaledDeltaTime, cancellationToken: SandWichManager.instance._source.Token);
            //클리어
            LaiseSandList.Clear();
            sandCnt = 0;
            await UniTask.Delay(TimeSpan.FromSeconds(SandWichManager.instance.gameSpeed), DelayType.UnscaledDeltaTime, cancellationToken: SandWichManager.instance._source.Token);
            // 샌드위치 쌓음  
            AllLaise();
        }
    }


}
