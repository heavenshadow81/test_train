using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Runtime.InteropServices;


public class SceneButton : MonoBehaviour
{
    [Header("씬 이름 선택")]
    [SerializeField] private SceneEnum sceneEnum;
    private Button button;
    private UIManager_PlayGround uiManager;
    

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager_PlayGround>();
        button = GetComponent<Button>();
        // 버튼이 null이 아닌지 확인
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    public void OnButtonClick()
    {
        button.interactable = false;
        SoundMGR.Instance.SoundPlay("PlayGround_Click3");
        SoundMGR.Instance.bgmSource.Stop();

        // UIFade 메서드에 씬 전환을 콜백으로 전달
        uiManager.UIFade(() =>
        {
            if(sceneEnum.ToString()== "00.PlayGroundMenu")
            {
                UIManager_PlayGround.pageNum = 0;
                UIManager.prevMenuNumber = 0;
            }

            SceneManager.LoadSceneAsync(sceneEnum.ToString());
        });
    }
}
