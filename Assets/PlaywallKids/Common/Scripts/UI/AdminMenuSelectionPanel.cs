using UnityEngine;
using System.Collections;

/// <summary>
/// 관리자 메뉴, 콘텐츠 메뉴 제어 클래스
/// </summary>
public class AdminMenuSelectionPanel : MonoBehaviour
{
    public static bool switchContentsManagement = false;
    public UISprite fadeOutSprite;
    public GameObject hiddenMenuButton;
    public UIGrid hiddenMenuGrid;
    public GameObject goContentsManagement;
    public string menu2DSceneName = "TwoDimensionDrawMode";
    public string menu3DSceneName = "DragonPark";
    public string menuTouchSceneName = "";
    public string menuInteractionSceneName = "";
    public string menuMotionSceneName = "";
    public string menuAquariumSceneName = "Aquarium";
    public string menuCommonMode = "DragonParkCommonSelectMenu";
    
    private AnimatablePanel _contentsManagement;
    private AnimatablePanel _printerManagement;

    void OnEnable()
    {
#if UNITY_EDITOR
        switchContentsManagement = true;
#endif

        // 관리자 모드 중 콘텐츠 관리 버튼 활성 비활성 옵션
        // 바로가기에 arguments -contents_management-enable 입력
        // LauncherController _ParseCommandLineArguments()에서 값 적용 함 또는 전처리 조건 문 CONTENTS_MANAGEMENT_ENABLE 입력
        if (goContentsManagement)
        {
            goContentsManagement.SetActive(switchContentsManagement);
            if (hiddenMenuGrid != null)
                hiddenMenuGrid.Reposition();
        }

        // 콘텐츠 관리창, 프린터 관리창이 떠있으면 비활성화
        // 비활성화한 오브젝트도 FindObjectOfType()로 찾을 수 있다.
        foreach (Transform tf in transform.parent)
        {
            if (_contentsManagement == null && tf.name.Contains("(ContentsManagement)"))
            {
                _contentsManagement = tf.GetComponent<AnimatablePanel>();
                if (_contentsManagement != null) _contentsManagement.gameObject.SetActive(false);
            }
            if (_printerManagement == null && tf.name.Contains("(PrinterManagement)"))
            {
                _printerManagement = tf.GetComponent<AnimatablePanel>();
                if (_printerManagement != null) _printerManagement.gameObject.SetActive(false);
            }
        }

        StartCoroutine(FadeInProcess());
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.None;
    }

    /// <summary>
    /// 숨겨진 관리자 메뉴를 스크립트로 띄울 수 있도록 합니다.
    /// 관리자 메뉴가 있는 씬이 SpaceIntro, BigBoardMainMenu 등 여러곳에 분포되어 있으므로
    /// 기능 일관성을 유지하기 위한 구현입니다.
    /// </summary>
    public void ShowHiddenMenu()
    {
        // 관리자 메뉴 버튼 클릭 시뮬레이트
        hiddenMenuButton.SetActive(true);
        hiddenMenuButton.SendMessage("OnClick");

        // 실제 메뉴 표시는 프리팹의 UIPlayTween 컴포넌트가 수행.
    }

    public void ShowContentsManagement()
    {
        if (_contentsManagement != null)
            _contentsManagement.Show();
    }

    public void ShowPrinterManagement()
    {
        if (_printerManagement != null)
            _printerManagement.Show();
    }

    public void Menu2D()
    {
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Drawing2D;
        StartCoroutine(_LoadLevelDelayed(menu2DSceneName, 2.0f));
    }

    public void Menu3D()
    {
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Drawing3D;
        StartCoroutine(_LoadLevelDelayed(menu3DSceneName, 2.0f));
    }

    public void MenuTouch()
    {
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Touch;
        //StartCoroutine (_LoadLevelDelayed (menuTouchSceneName, 2.0f));
        StartCoroutine(_LoadLevelDelayed(menuCommonMode, 2.0f));
    }

    public void MenuInteraction()
    {
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Interaction;
        //StartCoroutine(_LoadLevelDelayed(menuCommonMode, 2.0f));

        StartCoroutine(_LoadLevelDelayed(menuInteractionSceneName, 2.0f));
    }

    public void MenuMotion()
    {
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Motion;
        StartCoroutine(_LoadLevelDelayed(menuCommonMode, 2.0f));

        //StartCoroutine (_LoadLevelDelayed (menuMotionSceneName, 2.0f));
    }

    public void MenuAquarium()
    {
        BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Aquarium;
        StartCoroutine(_LoadLevelDelayed(menuAquariumSceneName, 2.0f));
    }

    IEnumerator FadeInProcess()
    {
        // Wait for seconds
        float t = 0.0f;
        float time = 0.5f;
        fadeOutSprite.cachedGameObject.SetActive(true);
        fadeOutSprite.alpha = 1.0f;

        while (t < time)
        {
            t += Time.deltaTime;
            fadeOutSprite.alpha -= t / time;

            yield return null;
        }

        fadeOutSprite.alpha = 0;
        fadeOutSprite.cachedGameObject.SetActive(false);

    }

    private IEnumerator _LoadLevelDelayed(string levelName, float time)
    {
        // Disable all children's colliders to prevent touch event.
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
            c.enabled = false;

        // Wait for seconds
        float t = 0.0f;
        fadeOutSprite.cachedGameObject.SetActive(true);

        while (t < time)
        {
            fadeOutSprite.alpha = t / time;

            yield return null;

            t += Time.deltaTime;
        }

        fadeOutSprite.alpha = 1.0f;

        yield return new WaitForSeconds(time);

        // Load level!
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }
}