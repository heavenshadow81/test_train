using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Bax.P0.Client.UnityWorld.PictureGame
{
    public class HiddenFindData : MonoBehaviour
    {
        //체크가 됬는지 확인
        public bool B_Check { get; set; }
        //동물이름으로 파일로드용도로 쓰는 열거형
        public FindStuff stuff;
        //이미지 랜더러
        public SpriteRenderer spRender;
        //콜라이더2D 충돌체
        public BoxCollider2D boxCollider2D;

        //컬라이더 크기 세팅
        public void CollSizeSet()
        {
            //컬리아더 크기를 이미지 크기에 맞게 조절
             boxCollider2D.size = new Vector3(spRender.sprite.bounds.size.x, spRender.sprite.bounds.size.y, 0);
        }


        public  void DownProcess()
        {   
            //게임 클리어 했거나 다음스테이지로 로딩중일때 막기
            if (HiddenGameMGR.Instance.GameClear || HiddenGameMGR.Instance.loadingState == HiddenGameMGR.LoadingState.Loading) return;
            if (B_Check == false && HiddenGameMGR.Instance.stateClass.state == GameState.GamePlay)
            {
                //체크
                B_Check = true;
                //체크한 갯수 증가
                HiddenGameMGR.Instance.checkIdx++;
                // 동그라미 생성
                var obj = HiddenGameMGR.Instance.pulling.GetObj();
                // 동그라미 리스트에 저장
                HiddenGameMGR.Instance.highlights.Add(obj);
                //정답 사운드
                SoundMGR.Instance.SoundPlay("04.정답");

                //부모 설정
                obj.transform.SetParent(HiddenGameMGR.Instance.pullingParent);
                //위치설정
                obj.transform.position = transform.position;
                //스케일 1
                obj.transform.localScale = Vector3.one;

                //우측 동물 중 같은 동물이름으로 저장되어 있는지 찾음 
                var ob = HiddenGameMGR.Instance.R.Find(x => x.stuff == stuff);
                //찾았다면 회색에서 흰색으로 변경
                ob.spriteRenderer.color = Color.white;
                
                // 동그라미 이미지 컴포넌트
                var FillImage = obj.GetComponent<Image>();
                //fill 값 0
                FillImage.fillAmount = 0;
                //컬라이더 비활성화
                boxCollider2D.enabled = false;
                
                //트윈으로 0 ~ 1 까지 증가 
                //동그라미 그려주는 효과 
                DOTween.To(() => FillImage.fillAmount, x => FillImage.fillAmount = x, 1, 1)
                .OnComplete(async () => 
                {
                    //마지막 스테이지
                    if (HiddenGameMGR.Instance.HiddenStage >= HiddenGameMGR.Instance.hiddenMaxStage && HiddenGameMGR.Instance.GameClear == false)
                    {
                        clearAsync(true);
                    }
                    else  //마지막 스테이지가 아니다
                    {
                        clearAsync(false);
                    }
                });
            }
        }


        //private void OnMouseDown()
        //{
        //    DownProcess();
        //}

        private async void clearAsync(bool Clear)
        { 
            //찾아야 할 이미지를 다 찾았다면 (5개)
            if (HiddenGameMGR.Instance.checkIdx >= HiddenGameMGR.Instance.MaxCheckIdx)
            { 
                if (Clear)
                {
                    //게임 클리어
                    HiddenGameMGR.Instance.GameClear = true;
                    //게임 결과 Success
                    HiddenGameMGR.Instance.stateClass.resultState = GameResult.Success;
                    //게임 Result 상태로 변환
                    HiddenGameMGR.Instance.zozo.Change(GameState.GameResult);
                }
                else
                {
                    // 체크 갯수 0
                    HiddenGameMGR.Instance.checkIdx = 0;
                    //스테이지 증가 
                    HiddenGameMGR.Instance.HiddenStage++;
                    //0.5f초 뒤
                    await UniTask.Delay(TimeSpan.FromSeconds(0.5f), DelayType.UnscaledDeltaTime);
                    //화면전환 사운드
                    SoundMGR.Instance.SoundPlay("04.화면전환");
                    //화면 전환로딩 Go 
                    HiddenGameMGR.Instance.StageChage(HiddenGameMGR.Instance.HiddenStage).Forget();
                    return;
                }
            }
        }

       
    }
}