using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager_PlayGround : MonoBehaviour
{
    public static GameManager_PlayGround Instance;

    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject robot;

    public Action OnGameStart;

    void Awake()
    {
        // Singleton 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 Instance가 존재하면 중복 생성 방지
        }

        GameObject soundManager = GameObject.Find("SoundMGR");
        if (soundManager != null)
        {
            soundManager.SetActive(false);
        }
    }

    void OnEnable()
    {
        CountDown.OnCountdownFinished += GameStart; // 이벤트 구독
    }

    void OnDisable()
    {
        CountDown.OnCountdownFinished -= GameStart; // 이벤트 구독
    }

    void GameStart()
    {
        if (CountDown.gameStart) // CountDown 클래스가 정의되어 있어야 함
        {
            if(robot != null)
            {
                robot.SetActive(true);
            }

            gameCanvas.SetActive(true);

            OnGameStart?.Invoke(); // null 체크 후 Invoke
        }
    }
}
