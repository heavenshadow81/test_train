using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_Mugunghwa : MonoBehaviour
{
    public static GameManager_Mugunghwa Instance;

    private Player_Mugunghwa player;

    public bool isGameOver;
    public bool isPlaying;

    public Button button;
    public TextMeshProUGUI retryText;

    void Awake()
    {
        Instance = this;
        player = FindObjectOfType<Player_Mugunghwa>();
    }

    public bool IsPlayerMoving()
    {
        return player.GetIsMoving();
    }

    public void GameOver()
    {
        // 게임 오버 처리
        player.gameOverUI.SetActive(true);
        player.spriteRenderer.sprite = player.sprites[2];
        SoundMGR.Instance.SoundStop("무궁화_걷기");
        SoundMGR.Instance.SoundPlay("무궁화_게임오버");
        isGameOver = true;
        GameStop();
    }

    public void GameStop()
    {
        isPlaying = false;
        player.isMoving = false;
        button.enabled = true;
        retryText.gameObject.SetActive(true);
    }
}
