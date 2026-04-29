using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    //지우개 이미지 랜더
    public SpriteRenderer Sprenderer;

    //지우개 버튼 입력
    public async void DownProcess()
    {
        //지우개 컬러 회색
        Sprenderer.color = Color.gray;
        //지우개 사운드 플레이
        SoundMGR.Instance.SoundPlay("21.지우개");
        //생성된 전체 스탬프비움
        foreach (var item in PaintMGR.Instance.stampList)
        {
            //제거
            item.ReturnObj();
        }
        //스탬프리스트 비움
        PaintMGR.Instance.stampList.Clear();
        //지우면 솔팅값  -100
        PaintMGR.Instance.sorting = -100;
        //딜레이 0.1초
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f) , DelayType.UnscaledDeltaTime);
        //컬러값 흰색으로 
        Sprenderer.color = Color.white;
    }
}
