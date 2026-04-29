using ML.MLBKids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectBackground : MonoBehaviour
{
    //임시로 만든 배경 선택 버튼
    [SerializeField]
    AudioSource effectsounds;
    public void GoToSelectBackground()
    {
        StartCoroutine(FadeOUt());
        SceneManager.LoadSceneAsync("BackgroundSelect");
    }

    public void GoToZooZoo()
    {
        StartCoroutine(GoZooZoo());
    }
    //소리 딜레이 추가
    IEnumerator GoZooZoo()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Drawing2D;
        SceneManager.LoadSceneAsync("Intro");
        if (Settings.instance != null && Settings.instance.gameObject != null)
        {
            Settings.instance.gameObject.SetActive(true);
        }
        yield break;
    }

    public void GoTo2D()
    {
        StartCoroutine(Go2D());
    }
    //소리 딜레이 추가
    IEnumerator Go2D()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Drawing2D;
        SceneManager.LoadSceneAsync(nameof(ContentsStoreItemType.Mode.Drawing2D));
        yield break;
    }
    public void GoToBubblePang()
    {
        StartCoroutine(GoBubblePang());
    }
    //소리 딜레이 추가
    IEnumerator GoBubblePang()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.BubblePang;
        SceneManager.LoadSceneAsync(nameof(ContentsStoreItemType.Mode.BubblePang));
        yield break;
    }
    public void GoToCoding()
    {
        StartCoroutine(GoCoding());
    }
    //소리 딜레이 추가
    IEnumerator GoCoding()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Coding;
        SceneManager.LoadSceneAsync(nameof(ContentsStoreItemType.Mode.Coding));
        yield break;
    }
    //드래곤 파크 이동
    public void GoToDragonPark()
    {
        StartCoroutine(GoDragonPark());
    }
    //소리 딜레이 추가
    IEnumerator GoDragonPark()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Drawing3D;
        SceneManager.LoadSceneAsync(nameof(ContentsStoreItemType.Mode.Drawing3D));
        yield break;
    }

    public void GoToTouch()
    {
        Stadium stadium = Stadium.instance;
        if (stadium != null)
        {
            stadium.Cleanup();
        }

        StartCoroutine(GoTouch());
    }
    IEnumerator GoTouch()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Touch;
        SceneManager.LoadSceneAsync(nameof(ContentsStoreItemType.Mode.Touch));
        yield break;
    }
    public void GoToInteraction()
    {
        StartCoroutine(GoInteraction());
    }
    IEnumerator GoInteraction()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Interaction;
        SceneManager.LoadSceneAsync(nameof(ContentsStoreItemType.Mode.Interaction));
        yield break;
    }
    public void GoToMotion()
    {
        StartCoroutine(GoMotion());
    }

    IEnumerator GoMotion()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Motion;
        SceneManager.LoadSceneAsync(nameof(ContentsStoreItemType.Mode.Motion));
        yield break;
    }

    public void GoToAquarium()
    {
        StartCoroutine(GoAquarium());
    }

    IEnumerator GoAquarium()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Aquarium;
        SceneManager.LoadSceneAsync(nameof(ContentsStoreItemType.Mode.Aquarium));
        yield break;
    }

    IEnumerator FadeOUt()
    {
        yield return new WaitForSeconds(0.1f);
        FadeInOutCamera fadecamera = new FadeInOutCamera();
        fadecamera.opacity += Time.deltaTime;
    }
    //
    public void GoToNTouch()
    {
        StartCoroutine(GoNTouch());
    }

    IEnumerator GoNTouch()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        SceneManager.LoadSceneAsync("SelectPuzzle");
        yield break;
    }
    //바꾸기
    public void GoScene(ContentsStoreItemType.Mode mode)
    {
        BigboardGlobal.currentMode = mode;
        SceneManager.LoadSceneAsync(nameof(mode));
    }

    public void RestartScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    //이젠 커뮤니케이션 과학 영상 재생
    public void GoToEzeneScience()
    {
        StartCoroutine(GoEzeneScience());
    }
    IEnumerator GoEzeneScience()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        SceneManager.LoadSceneAsync("SmartPangEzene");
        yield break;

    }
    //이젠 커뮤니케이션 영어 영상 재생
    public void GoToEzeneEnglish()
    {
        StartCoroutine(GoEzeneEnglish());
    }

    IEnumerator GoEzeneEnglish()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        SceneManager.LoadSceneAsync("SmartPangEzeneEnglish");
        yield break;
    }
    //종료
    public void Quit()
    {
        Application.Quit();
    }
    //퍼즐 선택 초기로 돌아가기...
    public void GoHome()
    {
        SceneManager.LoadSceneAsync("SelectPuzzle");
    }
    //스마트팡 기본 홈으로 이동
    public void GoToSmartPangHome()
    {
        StartCoroutine(GoSmartPangHome());
    }

    IEnumerator GoSmartPangHome()
    {
#if KINECTPANG
        Application.Quit();
#endif
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !effectsounds.isPlaying);
        SceneManager.LoadSceneAsync("SmartPangMenu");
        yield break;
    }
}
