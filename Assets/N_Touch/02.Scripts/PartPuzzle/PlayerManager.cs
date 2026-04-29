using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
//플레이어 UI담당 매니저
public class PlayerManager : MonoBehaviour
{
    #region 변수 및 프로퍼티
    //테마 번호, 해당 테마의 동물 숫자
    [SerializeField]
    int theme, animalnumber, animalindex;
    //해당 테마에 해당되는 트랜스폼 활성화
    public Transform thisTheme;

    //맞추어야 할 파트 조각(찾아야 할 그림자 조각)
    [SerializeField]
    Image partImage;
    //어느 부분인가?(부분 퍼즐)
    List<int> parts = new List<int>();
    int part;

    [SerializeField]
    GameObject ResultImage;
    [SerializeField]
    //플레이어 이름
    Text playerName;

    //딜레이 측정 시간
    float time = 0; //임재성님 코드
    //완성되면 이동할 그림 관련 변수
    //부분 퍼즐 조각에 쓰일 것
    public GameObject moveImagePrefabs;
    public Transform gameUI;
    public Transform bgAnimalPos;
    public BGAnimalManager bganimalManager;
    //그림자 퍼즐
    public Transform shadowBtnGroups;
    public GameObject testPrefab;
    public Sprite[] shadowMoveImage;

    //그림자 퍼즐 관련 스프라이트
    [SerializeField]
    Sprite[] shadowImages;

    //랜덤 배치를 위한 배열
    int[] shufflepart;

    //그림자 선택된 것 확인용
    Dictionary<int, bool> selected = new Dictionary<int, bool>();

    //맞게? 틀리게?
    [SerializeField]
    Image tfimage;
    //동물 이름
    [SerializeField]
    Text animalName;

    int actionCount;


    // 20221118 추가
    public Image settingBtn;
    public bool settingBtnActive
    {
        set
        {   
            settingBtn.raycastTarget = value;
        }
    }
    #endregion

    #region 이벤트
    //초기화 관련 이벤트 설정
    event System.EventHandler InitalizePuzzle;


    //이벤트 실행 함수
    void InitialPuzzle()
    {
        if (this.InitalizePuzzle != null)
        {
            InitalizePuzzle(this, System.EventArgs.Empty);
        }
    }
    #endregion


    #region 함수

    #region 공통
    //파일 불러오기
    void Awake()
    {
        settingBtn = GameObject.Find("LevelSettingBtn").GetComponent<Image>();
        theme = (int)TouchContentsManger.Instance.contents;
        //이벤트 설정
        switch (TouchContentsManger.Instance.contentsType)
        {
            case TouchType.Part:
                //부분 퍼즐 동물 테마 지정
                theme++;
                SetPartTheme();
                //부분퍼즐 이벤트 설정
                InitalizePuzzle += InitailPart;

                break;
            case TouchType.Shadow:
                //그림자 동물 테마 지정
                ShadowImageSet(theme);
                thisTheme = transform.GetChild(1);
                //그림자 이벤트 설정
                InitalizePuzzle += InitialShadow;

                break;
        }

    }

    //초기화
    public void Init()
    {
        //결과창 비활성화
        if (ResultImage.activeSelf)
        {
            ResultImage.SetActive(false);
        }

        if (thisTheme == null)
        {
            int ani = (TouchContentsManger.Instance.contentsType == TouchType.Part) ? theme : 0;
            thisTheme = transform.GetChild(ani);
        }
        //랜덤하게 만들기
        List<int> animalpart = new List<int>();
        for (int i = 0; i < thisTheme.childCount; i++)
        {
            animalpart.Add(i);
        }
        //섞어서
        shufflepart = ShuffleArray(animalpart.ToArray(), Random.Range(0, 60));
        theme = (int)TouchContentsManger.Instance.contents;
        //해당 동물 선택!
        animalindex = 0;
        //animalnumber = shufflepart[animalindex];
        actionCount = 0;
        InitialPuzzle();
    }
    //힌트용!!!
    public void Hint()
    {
        StartCoroutine(Blink());
    }
    //버튼깜빡임
    IEnumerator Blink()
    {
        int whatchange = (TouchContentsManger.Instance.contentsType == TouchType.Part) ? part : System.Array.IndexOf(shufflepart, animalnumber);
        while (true)
        {
            ChangeColorInstance(whatchange, 0);
            yield return new WaitForSeconds(0.1f);
            ChangeColorInstance(whatchange, 255);
            yield return new WaitForSeconds(0.1f);
        }
    }
    //비교용..
    public Image GetPartImage() => partImage;
    #endregion

    #region 조각 퍼즐

    //테마 고르기
    public void SetPartTheme()
    {
        for (int i = 1; i < transform.childCount - 2; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == theme);
        }
        thisTheme = transform.GetChild(theme);
        print("테마 설정 완료");
    }

    //버튼 한 동물만 활성화
    public void SetPartButton()
    {

        for (int i = 0; i < thisTheme.childCount; i++)
        {
            //해당 동물 맞는지 확인

            //각각의 상태대로 활성//
            thisTheme.GetChild(i).gameObject.SetActive(i == animalnumber);
        }
    }

    //처음 시작시 이미지 빛깔 바꾸기(그림자 처리, 혹은 밝게)
    public void ChangeColor(int buttonindex, int colornumber)
    {
        switch (TouchContentsManger.Instance.contentsType)
        {
            case TouchType.Part:
                thisTheme.GetChild(animalnumber).GetChild(buttonindex).GetComponent<TouchCustomShape>().SetColor(colornumber);
                break;
            case TouchType.Shadow:
                thisTheme.GetChild(buttonindex).GetComponent<TouchCustomShape>().SetColor(colornumber);
                break;
        }
    }
    //즉각 바꾸기!!
    public void ChangeColorInstance(int buttonindex, int colornumber)
    {
        switch (TouchContentsManger.Instance.contentsType)
        {
            case TouchType.Part:
                thisTheme.GetChild(animalnumber).GetChild(buttonindex).GetComponent<TouchCustomShape>().SetColorInstance(colornumber);
                break;
            case TouchType.Shadow:
                thisTheme.GetChild(buttonindex).GetComponent<TouchCustomShape>().SetColorInstance(colornumber);
                break;
        }
    }

    //동물 퍼즐 초기화 부분
    void InitailPart(object sender, System.EventArgs e)
    {
        //섞는 것 공통으로....
        shufflepart = TouchContentsManger.Instance.shufflepart;
        animalnumber = shufflepart[animalindex];
        parts.Clear();
        for (int i = 0; i < thisTheme.GetChild(animalnumber).childCount; i++)
        {
            //모두 까맣게
            ChangeColor(i, 0);
            parts.Add(i);
        }

        SetPartButton();

        //랜덤하게 골라서..!
        part = Random.Range(0, parts.Count);
        partImage.sprite = thisTheme.GetChild(animalnumber).GetChild(part).GetComponent<Image>().sprite;
        partImage.rectTransform.localScale = thisTheme.transform.GetChild(animalnumber).localScale;
        partImage.SetNativeSize();
        partImage.enabled = true;
        //동물 이름 초기화
        animalName.text = "";
    }
    //동물 퍼즐 버튼 부분에서 사용할 함수
    //맞는 그림을 골랐으면...?
    public void SelectImage(int partnumber)
    {
        StopAllCoroutines();
        //맞는 그림이라면..?//조건 하나 더 추가..
        if (partnumber == part)
        {
            //밝게 바꾸고
            ChangeColor(part, 255);

            //해당 파트 제거
            if (parts.Contains(part))
            {
                parts.Remove(part);
            }
            part = 255;//딜레이 넣었을 때 겹침 방지

            settingBtnActive  = false;

            //이 부분 딜레이 추가 및 동그라미 이펙트 추가...
            tfimage.sprite = TouchContentsManger.Instance.resultoxImage[0];
            tfimage.GetComponent<CorrectImageSet>().aTime = 0.5f;
            //이 부분에 소리 맞음 추가
            tfimage.GetComponent<AudioSource>().clip = TouchContentsManger.Instance.SetTFClip(true);
            tfimage.gameObject.SetActive(true);
            //파트 이미지 비우기
            partImage.enabled = false;
            //새로 넣기..
            if (parts.Count > 0)
            {
                //딜레이필요!!!새 파트로 바꾸기
                Invoke(nameof(DelaySetPart), 0.5f);
                //part = parts.ToArray()[Random.Range(0, parts.Count)];
                //partImage.sprite = thisTheme.GetChild(animalnumber).GetChild(part).GetComponent<Image>().sprite;
            }
            else
            {
                //조각 다 맞추면..?
                //Invoke로 늦게 호출하기
                part = 255;//더 눌리지 않게!


                // 20221118 추가
                settingBtnActive = false;

                //임재성님의 코드:완성된 동물 그림 이동 작업(랜덤 위치)
                //중복 확인 메소드
                bool temp = bganimalManager.DuplicationCheck(animalnumber);

                //동물 생성(리팩토링 예정)
                if (!temp)
                {
                    GameObject go = Instantiate(moveImagePrefabs);
                    go.transform.SetParent(thisTheme.GetChild(animalnumber).transform.parent);
                    go.transform.SetParent(gameUI);
                    go.GetComponent<Image>().sprite = bganimalManager.GetComponent<BGAnimalManager>().animalsImg[animalnumber];
                    go.transform.name = go.GetComponent<Image>().sprite.name;
                    print(go.transform.name);
                    go.transform.localPosition = new Vector2(0, 0);//Badcode
                    go.transform.localScale = new Vector2(4.5f, 4.5f);//Badcode
                    go.GetComponent<MoveImg>().Move();
                }
                //소리
                TouchContentsManger.Instance.AudioPlay(animalnumber);
                animalName.text = TouchContentsManger.Instance.AnimalNameSet(animalnumber);
                //
                Invoke(nameof(DelaySecond), 2); //임재성님 작성


                ////랜덤 애니멀 셔플된 것(늘리기)
                //if (animalindex < shufflepart.Length - 1)
                //{
                //    animalindex++;
                //    animalnumber = shufflepart[animalindex];
                //    InitialPuzzle();
                //}
                ////만약 지정된 것 다 썼으면...?
                //else
                //{
                //    if (playerName.text.Length > 0)
                //    {
                //        int range = Mathf.Min(3, playerName.text.Length);
                //        for (int i = 0; i < range; i++)
                //        {
                //            ResultImage.transform.GetChild(i).GetComponentInChildren<Text>().text = playerName.text[i].ToString();
                //        }
                //    }
                //    ResultImage.SetActive(true);
                //}


            }
        }
        //조각이 맞지 않는 상황이라면...?
        else
        {
            //X이펙트 생성
            tfimage.sprite = TouchContentsManger.Instance.resultoxImage[1];
            tfimage.GetComponent<CorrectImageSet>().aTime = 2f;
            //이 부분에 틀림 소리 추가
            tfimage.GetComponent<AudioSource>().clip = TouchContentsManger.Instance.SetTFClip(false);
            tfimage.gameObject.SetActive(true);
        }
        actionCount++;
    }

    //시간 딜레이 함수 부분 그림 바꾸기
    void DelaySetPart()
    {
        part = parts.ToArray()[Random.Range(0, parts.Count)];
        partImage.sprite = thisTheme.GetChild(animalnumber).GetChild(part).GetComponent<Image>().sprite;
        partImage.rectTransform.localScale = thisTheme.transform.GetChild(animalnumber).localScale;
        partImage.SetNativeSize();
        partImage.enabled = true;
    }

    //시간 딜레이 함수 -임재성님 동물퍼즐 동물그림 바꾸기
    void DelaySecond()
    {
        print($"animalindex : {animalindex}\nshufflepart 길이 : {shufflepart.Length - 1}");
        if (animalindex < shufflepart.Length - 1)
        {
            animalindex++;
            animalnumber = shufflepart[animalindex];
            InitialPuzzle();
        }
        //만약 지정된 것 다 썼으면...? > 동물 퍼즐 콘텐츠 완료!!!>
        else
        {
            
            if (playerName.text.Length > 0)
            {
                int range = Mathf.Min(3, playerName.text.Length);
                for (int i = 0; i < range; i++)
                {
                    ResultImage.transform.GetChild(i).GetComponent<Text>().text = playerName.text[i].ToString();
                    ResultImage.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    ResultImage.transform.GetChild(i).GetComponent<Text>().text = "";
                    ResultImage.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            //NFC관련
            //점수 계산 여기에 넣기!!!


            //int difficultaddition = transform.parent.name.Contains("Easy") ? 4 : 6;
            //print($"플레이어 버튼 조작 횟수: {actionCount}\n최소 조작 횟수: {shufflepart.Length * difficultaddition}");
            //float ratio = shufflepart.Length * difficultaddition / actionCount;
            //int score = (int)(ratio * 100);
            //if (playerName.text.Length > 0)
            //{
            //    var nfcinformation = FindObjectOfType<NFCInformation>();
            //    if (nfcinformation)
            //    {
            //        var nfcnameset = nfcinformation.nameSet.GetComponent<NFCUINameSet>();
            //        foreach (var i in nfcnameset.carduidsNameKeyValues)
            //        {
            //            if (i.Value == playerName.text)
            //            {
            //                nfcinformation.contentsScore[i.Key] = score;
            //                print(nfcinformation.ResponseName(i.Key, 0));
            //                print(score);
            //                ResultImage.SetActive(true);
            //            }
            //            else
            //            {
            //                print(nfcinformation.ResponseName(i.Key, 0));
            //            }
            //        }
            //    }
            //}
            //else
            //{
                ResultImage.SetActive(true);
            //}
        }
    }


    #endregion

    #region 그림자 퍼즐

    //그림자 퍼즐 이미지 선택함수
    void ShadowImageSet(int theme)
    {
        shadowImages = theme switch
        {
            //동물
            0 => TouchContentsManger.Instance.shadowmammalImages,
            //곤충
            1 => TouchContentsManger.Instance.shadowInsectImages,
            //공룡
            2 => TouchContentsManger.Instance.shadowdinoImages,
            //바다
            3 => TouchContentsManger.Instance.shadowoceanImages
        };
    }
    //그림자 퍼즐 초기화
    void InitialShadow(object sender, System.EventArgs e)
    {
        if (animalindex == 0)
        {
            List<int> shadowpart = new List<int>();
            for (int i = 0; i < shadowImages.Length; i++)
            {
                shadowpart.Add(i);
            }

            //랜덤으로 배열 섞기
            shufflepart = ShuffleArray(shadowpart.ToArray(), Random.Range(0, 30)).Take((int)TouchContentsManger.Instance.Difficulty * 2 + 6).ToArray();
            //동물 숫자 지정
            animalnumber = shufflepart[animalindex];
            selected.Clear();
            actionCount = 0;
            //해당 버튼 설정(섞기 반영)
            for (int i = 0; i < thisTheme.childCount; i++)
            {
                thisTheme.GetChild(i).GetComponent<TouchCustomShape>().SetColor(0);
                thisTheme.GetChild(i).GetComponent<Button>().image.sprite = shadowImages[shufflepart[i]];
                selected.Add(shufflepart[i], false);//사전 초기화
            }
            //사전 초기화
            //for(int i = 0; i<shufflepart.Length; i++)
            //{
            //    selected.Add(shufflepart[i], false);//사전 모두 초기화.. 선택된 적 없음
            //}
        }
        else
        {
            int[] temp = shufflepart;
            shufflepart = ShuffleArray(temp, Random.Range(0, 30));
            for (int i = 0; i < thisTheme.childCount; i++)
            {
                int colorset = (selected[shufflepart[i]]) ? 255 : 0;
                thisTheme.GetChild(i).GetComponent<TouchCustomShape>().SetColorInstance(colorset);
                thisTheme.GetChild(i).GetComponent<Button>().image.sprite = shadowImages[shufflepart[i]];
            }
        }
        partImage.rectTransform.localScale = thisTheme.transform.GetChild(System.Array.IndexOf(shufflepart, animalnumber)).localScale;
        partImage.enabled = true;
        partImage.sprite = shadowImages[animalnumber];
    }

    //배열 나누기


    //배열 랜덤으로 섞기
    public T[] ShuffleArray<T>(T[] array, int seed)
    {
        //랜덤한 값 얻기....
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
        
    //그림자 퍼즐에서 실행할 것
    public void SelectShadowImage(int shadownumber)
    {
        if (shufflepart[shadownumber] == animalnumber)
        {
            StopAllCoroutines();
            //밝게 바꾸기
            ChangeColor(shadownumber, 255);
            selected[animalnumber] = true;

            //소리 재생!
            TouchContentsManger.Instance.AudioPlay(animalnumber);

            //딜레이 두기
            animalnumber = 255;//중복 선택 방지
            //맞음 틀림 표시

            // 20221118 추가
            settingBtnActive = false;
            //

            tfimage.sprite = TouchContentsManger.Instance.resultoxImage[0];//동그라미 표시
            tfimage.GetComponent<CorrectImageSet>().aTime = 0.5f;
            tfimage.GetComponent<AudioSource>().clip = TouchContentsManger.Instance.SetTFClip(true);
            tfimage.gameObject.SetActive(true);//활성화

            //그림자 선택시 보여줄 그림 비우기
            partImage.enabled = false;
            //그림자 활성화된 동물 작업// 한번에 다 같이 등장//위치는 랜덤으로.....
            //
            Invoke("DelayShadow", 0.5f);
            //선택되었음!

        }
        else
        {
            //틀렸다면...
            //X이펙트 추가 
            tfimage.sprite = TouchContentsManger.Instance.resultoxImage[1];//가새 표시
            tfimage.GetComponent<CorrectImageSet>().aTime = 2f;
            //partImage.transform.GetChild(0).transform.position = thisTheme.GetChild(shadownumber).transform.position;//위치를 옮기고 
            //틀린 소리 추가
            tfimage.GetComponent<AudioSource>().clip = TouchContentsManger.Instance.SetTFClip(false);
            tfimage.gameObject.SetActive(true);

        }
        actionCount++;
    }
    //시간 지연을 위해...
    void DelayShadow()
    {

        if (animalindex < shufflepart.Length - 1)
        {
            animalindex++;
            List<int> temp = new List<int>();
            foreach (var p in selected.Where(w => w.Value == false).Select(s => s.Key))
            {
                temp.Add(p);
            }
            animalnumber = temp[Random.Range(0, temp.Count)];
            InitialPuzzle();
            temp.Clear();
        }
        else
        {
            //모두 완성되었을 때            
            CompleteShadowPuzzle();//임재성님의 코드

            if (playerName.text.Length > 0)
            {
                int range = Mathf.Min(3, playerName.text.Length);
                for (int i = 0; i < range; i++)
                {
                    ResultImage.transform.GetChild(i).GetComponent<Text>().text = playerName.text[i].ToString();
                    ResultImage.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    ResultImage.transform.GetChild(i).GetComponent<Text>().text = "";
                    ResultImage.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            ResultImage.SetActive(true);
        }
    }
    //ljs----------------------------------------------------start
    void CompleteShadowPuzzle()
    {
        // Take Random position 
        //int[] ranArr;
        //int ranLength = shadowBtnGroups.childCount;
        //ranArr = Enumerable.Range(0, ranLength).ToArray();
        //for (int i = 0; i < ranLength; ++i)
        //{
        //    int ranIdx = Random.Range(i, ranLength);
        //    int tmp = ranArr[ranIdx];
        //    ranArr[ranIdx] = ranArr[i];
        //    ranArr[i] = tmp;
        //}

        GameObject.Find("BGAnimalPos").GetComponent<BGAnimalPos>().SetPosition(shadowBtnGroups.childCount);

        GameObject[] shadowBtns = new GameObject[shadowBtnGroups.childCount];
        //같아서 임의로 추가한 부분(에디터 상에서 끌어올 필요 없음) - 이슬기찬씨
        shadowMoveImage = shadowImages;

        //if (GameObject.Find("MoveImgGroup").transform.childCount <= shadowBtnGroups.childCount)
        //{
        for (int i = 0; i < shadowBtnGroups.childCount; i++)
        {
            // 이미지 생성
            GameObject go = Instantiate(testPrefab);
            for (int j = 0; j < shadowMoveImage.Length; j++) //badcode
            {
                if (shadowBtnGroups.GetChild(i).GetComponent<Button>().GetComponent<Image>().sprite.name == shadowMoveImage[j].name)
                {
                    go.GetComponent<Image>().sprite = shadowMoveImage[j];
                    go.transform.name = go.GetComponent<Image>().sprite.name;
                }
            }
            go.transform.SetParent(GameObject.Find("MoveImgGroup").transform);//badcode
            go.transform.position = shadowBtnGroups.GetChild(i).transform.position;
            //go.GetComponent<MoveImg_Shadow>().Move(ranArr[i]);
            go.GetComponent<MoveImg_Shadow>().MovePos(go.transform.name);
        }
        //}
        int difficultaddition = transform.parent.name.Contains("Easy") ? 6 : 8;
        float ratio = shufflepart.Length * difficultaddition / actionCount;
        int score = (int)(ratio * 100);
        //if (playerName.text.Length > 0)
        //{
        //    var nfcinformation = FindObjectOfType<NFCInformation>();
        //    if (nfcinformation)
        //    {
        //        var nfcnameset = nfcinformation.nameSet.GetComponent<NFCUINameSet>();
        //        foreach (var i in nfcnameset.carduidsNameKeyValues)
        //        {
        //            if (i.Value == playerName.text)
        //            {
        //                nfcinformation.contentsScore[i.Key] = score;
        //                print(nfcinformation.ResponseName(i.Key, 0));
        //                print(score);
        //                ResultImage.SetActive(true);
        //            }
        //            else
        //            {
        //                print(nfcinformation.ResponseName(i.Key, 0));
        //            }
        //        }
        //    }
        //}
    }
    //ljs----------------------------------------------------end


    #endregion

    #endregion
}
