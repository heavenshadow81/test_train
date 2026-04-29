using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_CT : MonoBehaviour
{
    public static GameManager_CT Instance;

    [Header("초기화")]
    public AnswerSettingsUI_CT answerSettingsUI;
    public CorrectAnswerUI_CT correctAnswerUI;
    public PlayerController_CT playerController;
    public Bone_CT[] bone_CTs;

    public GameObject successUI;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Init()
    {
        playerController.SetFillZero();
        answerSettingsUI.Init();
        correctAnswerUI.Init();
        MoveBone();
    }

    public void GameSuccess()
    {
        StartCoroutine(GameSuccessRoutine());
    }

    IEnumerator GameSuccessRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        SoundMGR.Instance.bgmSource.Stop();
        SoundMGR.Instance.SoundPlay("CatchingText_Ending");
        successUI.SetActive(true);
    }

    public void StopBone()
    {
        foreach (Bone_CT bone in bone_CTs)
        {
            bone.StopBone();
        }
    }

    public void MoveBone()
    {
        foreach (Bone_CT bone in bone_CTs)
        {
            bone.Init();
        }
    }
}
