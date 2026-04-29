using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class IntroProcess : MonoBehaviour
{
    public GameObject JoinPanel;
    public Button joinBtn;
    public bool isAddressCheck = false; // PC인증 될때동안 joinBtn 잠가 놓기
    public GameObject JoinToTitleSelectPanel;

    //타이틀 입장시 변할 이미지들
    public Image[] waitToJoin;
    public GameObject[] TitlePanels;

    [NonSerialized] public bool isJoin = false;

    //이미지 로드
    public LoadSprite loadSprite;

    //매직랜드 좌버튼
    public Button prevBtnMagic;
    //매직랜드 우버튼
    public Button nextBtnMagic;
    //주토피아 좌버튼
    public Button prevBtnZoo;
    //주토피아 우버튼
    public Button nextBtnZoo;
    //어트랙션 좌버튼
    public Button prevBtnAtt;
    //어트랙션 우버튼
    public Button nextBtnAtt;
    //컨텐츠버튼들 타이틀 부모오브젝트
    public GameObject[] titleContentPanel;

    public Scene LoadScene;

    public RectTransform rect;

    public virtual void Prev()
    {
        if(Settings.instance.pageIdx != 0) SoundMGR.Instance.SoundPlay("0.좌버튼");

        if (Settings.instance.pageIdx - 1 <= 0)
        {
            Settings.instance.pageIdx = 0;
            //btn 비활성
            switch (Settings.instance.IntroState)
            {
                case SceneChangeState.MagicLand  : prevBtnMagic.interactable = false;  break;
                case SceneChangeState.Zootopia   : prevBtnZoo.interactable = false;    break;
                case SceneChangeState.Attraction : prevBtnAtt.interactable = false;    break;
            }
        }
        else if(Settings.instance.pageIdx - 1 >= 0) 
        {
            Settings.instance.pageIdx -= 1;
            switch (Settings.instance.IntroState)
            {
                case SceneChangeState.MagicLand  : prevBtnMagic.interactable = true; break;
                case SceneChangeState.Zootopia   : prevBtnZoo.interactable = true; break;
                case SceneChangeState.Attraction: prevBtnAtt.interactable = true; break;
            }
        }

        switch (Settings.instance.IntroState)
        {
            case SceneChangeState.MagicLand  : nextBtnMagic.interactable = true; break;
            case SceneChangeState.Zootopia   : nextBtnZoo.interactable = true; break;
            case SceneChangeState.Attraction: nextBtnAtt.interactable = true; break;
        }
        
        rect.DOAnchorPosX(-1920f * Settings.instance.pageIdx, 1);

    }

    public void RectMenuPosition(int idx)
    {
        rect.anchoredPosition = new Vector2(-1920f * idx, 0f);
        if (-1920f * idx == 0)
        {
            //왼쪽잠금
            switch (Settings.instance.IntroState)
            {
                case SceneChangeState.MagicLand:
                    prevBtnMagic.interactable = false;
                    nextBtnMagic.interactable = true;
                    break;
                case SceneChangeState.Zootopia:
                    prevBtnZoo.interactable = false;
                    nextBtnZoo.interactable = true;
                    break;
                case SceneChangeState.Attraction:
                    prevBtnAtt.interactable = false;
                    nextBtnAtt.interactable = true;
                    break;
            }
        }
        else if (-1920f * idx == ancherX) //-5760
        {
            //왼쪽 킴 오른쪽 잠금
            switch (Settings.instance.IntroState)
            {
                case SceneChangeState.MagicLand:
                    prevBtnMagic.interactable = true;
                    nextBtnMagic.interactable = false;
                    break;
                case SceneChangeState.Zootopia:
                    prevBtnZoo.interactable = true;
                    nextBtnZoo.interactable = false;
                    break;
                case SceneChangeState.Attraction:
                    prevBtnAtt.interactable = true;
                    nextBtnAtt.interactable = false;
                    break;
            }
        }
        else
        {
            switch (Settings.instance.IntroState)
            {
                case SceneChangeState.MagicLand:
                    prevBtnMagic.interactable = true;
                    nextBtnMagic.interactable = true;
                    break;
                case SceneChangeState.Zootopia:
                    prevBtnZoo.interactable = true;
                    nextBtnZoo.interactable = true;
                    break;
                case SceneChangeState.Attraction:
                    prevBtnAtt.interactable = true;
                    nextBtnAtt.interactable = true;
                    break;
            }
        }
    }
    

    public virtual void Next()
    {
        if (Settings.instance.pageIdx != Settings.instance.pageMax) SoundMGR.Instance.SoundPlay("0.우버튼");

        if (Settings.instance.pageIdx + 1 >= Settings.instance.pageMax)
        {
            Settings.instance.pageIdx = Settings.instance.pageMax;
            switch (Settings.instance.IntroState)
            {
                case SceneChangeState.MagicLand  : nextBtnMagic.interactable = false; break;
                case SceneChangeState.Zootopia   : nextBtnZoo.interactable = false; break;
                case SceneChangeState.Attraction : nextBtnAtt.interactable = false; break;
            }
        }
        else if (Settings.instance.pageIdx + 1 < Settings.instance.pageMax)
        {
            SoundMGR.Instance.SoundPlay("0.우버튼");
            Settings.instance.pageIdx += 1;
            switch (Settings.instance.IntroState)
            {
                case SceneChangeState.MagicLand  : nextBtnMagic.interactable = true; break;
                case SceneChangeState.Zootopia   : nextBtnZoo.interactable = true; break;
                case SceneChangeState.Attraction : nextBtnAtt.interactable = true; break;
            }
        }
        switch (Settings.instance.IntroState)
        {
            case SceneChangeState.MagicLand  : prevBtnMagic.interactable = true; break;
            case SceneChangeState.Zootopia   : prevBtnZoo.interactable = true; break;
            case SceneChangeState.Attraction : prevBtnAtt.interactable = true; break;
        }
        rect.DOAnchorPosX(-1920f * Settings.instance.pageIdx, 1);
       
    }


    public virtual void Awake()
    {
        Time.timeScale = 1;
    }


    public virtual void Start()
    {
        loadSprite = new LoadSprite("Intro");
        SoundMGR.Instance.bgmSource.Play();
    }
    //입장 버튼 이벤트
    public  async void JoinButton()
    {
        if (isAddressCheck)
        {
            SoundMGR.Instance.SoundPlay("0.입장");
            isJoin = true;
            //버튼(배) -40 만큼 아래로 트윈
            await joinBtn.transform.DOLocalMoveY(joinBtn.transform.localPosition.y - 40f,1);
            //Join 패널 비활성화
            JoinPanel.SetActive(false);
            //타이틀 선택 패널 활성화
            JoinToTitleSelectPanel.SetActive(true);
            joinBtn.transform.localPosition = new Vector3(joinBtn.transform.localPosition.x, -390f, joinBtn.transform.localPosition.z);

            Settings.instance.IntroState = SceneChangeState.Title;
            Settings.instance.introChange = IntroChange.Title;
        }
    }
    
    public virtual async void TitleJoinButton(int idx)
    {
        SoundMGR.Instance.SoundPlay("0.타이틀입장");
        isJoin = false;
        //타이틀 번호 저장
        Settings.instance.titleIdx = idx;
        //입장이미지 로드
        await loadSprite.LoadSpriteData("join", waitToJoin[idx]);

        await UniTask.Delay(TimeSpan.FromSeconds(1));
        //선택된 타이틀 패널 활성화
        TitlePanels[idx].gameObject.SetActive(true);
        //버튼들 UI 패널 입장시 페이지 idx 초기화
        Settings.instance.pageIdx = 0;
        //게임 선택 스테이트로 저장
        Settings.instance.introChange = IntroChange.Game;
        Settings.instance.IntroState = (SceneChangeState)idx + 2;
        //좌우 버튼 클릭시 이동시킬 패널 저장
        rect = (RectTransform)titleContentPanel[idx].transform;

        
        //최대 페이지 번호 저장
        //최대 x 값 저장
        switch (Settings.instance.IntroState)
        {
            case SceneChangeState.MagicLand:
                Settings.instance.pageMax = 3;
                ancherX = -5760;
                break;
            case SceneChangeState.Zootopia:
                Settings.instance.pageMax = 2;
                ancherX = -5760;
                break;
            case SceneChangeState.Attraction:
                Settings.instance.pageMax = 1;
                ancherX = -3840;
                break;
        }
    }
    int ancherX = 0;
}
