using UnityEngine;

public class CountDownTimer_CC : CountdownTimer
{
    [Header("½ÇÆÐ")]
    [SerializeField] GameObject failUI;
    [SerializeField] GameObject gameMap;


    protected override void OnTimerEnd()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Fail");
        failUI.SetActive(true);
        gameMap.SetActive(false);
    }
}
