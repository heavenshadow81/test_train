using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    static ChangeScene _instance;
    public static ChangeScene Instance { get => _instance; }

    [SerializeField]
    [Tooltip("Scene 목록")]
    GameObject[] scenes;

    [SerializeField]
    GameObject settingButton;

    [SerializeField]
    GameObject emptyObject;
    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
        }

        //ContenetStart();

    }

    // 초기화 - StartScene만 True, 나머지는 모두 False
    void ContenetStart()
    {
        SetActiveSettingButton(true);

        scenes[0].SetActive(true);
        scenes[1].SetActive(false);
        scenes[2].SetActive(false);
        scenes[3].SetActive(false);
    }

    private void SetActiveSettingButton(bool value)
    {
        if (settingButton == null) { return; }
        settingButton.SetActive(value);
    }

    // 캐릭터 선택 씬
    public void CharacterSceneLoad()
    {
        SetActiveSettingButton(false);

        scenes[0].SetActive(false);
        scenes[1].SetActive(true);
    }

    // 게임 진행 씬
    public void GameSceneLoad()
    {
        SetActiveSettingButton(false);

        scenes[1].SetActive(false);
        scenes[2].SetActive(true);
        emptyObject.SetActive(true);

    }

    // 결과 씬
    public void ResultSceneLoad()
    {
        SetActiveSettingButton(false);

        scenes[2].SetActive(false);
        scenes[3].SetActive(true);
    }
}
