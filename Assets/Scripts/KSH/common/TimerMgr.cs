using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

using Cysharp.Threading.Tasks;

public class TimerMgr 
{
    private TextMeshProUGUI timerText;
    public float time;
    private Action action = () => { };
    private EnumClass stateClass;

    public float late;


    private bool type = false;

    public TimerMgr(float _time, TextMeshProUGUI _timerText , 
        Action _action = null , EnumClass _stateClass = null , bool Type = true)
    { 
        timerText = _timerText;
        time = _time;
        action = _action;
        stateClass = _stateClass;
        type = Type;

        
        Init();
       
    }

    public void Init()
    {
        if (type == true)
            timer().Forget();
        else
            timerSuccess().Forget();
    }
    

    /// <summary>
    /// 시간이 0이 되면 종료
    /// 최대 시간 ~ 0 이 되면 종료
    /// </summary>
    /// <returns></returns>
    private async UniTask timer()
    {
        await UniTask.Yield();
        late = time;
        while ((late -= Time.deltaTime) >= 0f)
        {
            await UniTask.Yield();
            timerText.text = MathF.Round(late).ToString();
          

            if (stateClass.state != GameState.GamePlay ||
                stateClass.resultState == GameResult.Success)
            return;
        }
        stateClass.resultState = GameResult.Fail;
       
        action?.Invoke();
    }



    private async UniTask timerSuccess()
    {
        await UniTask.Yield();
        late = time;
        while ((late -= Time.deltaTime) >= 0f)
        {
            await UniTask.Yield();
            timerText.text = MathF.Round(late).ToString();
            
        }
        stateClass.resultState = GameResult.Success;

        action?.Invoke();
    }


    /// <summary>
    /// 시간 버티기 0 ~ 늘어남
    /// </summary>
    /// <returns></returns>
    private async UniTask timer2()
    {
        await UniTask.Yield();
        late = time;

        while (true)
        {
            await UniTask.Yield();
            timerText.text = MathF.Round(late += Time.deltaTime).ToString();

            if (stateClass.resultState == GameResult.Fail || 
                stateClass.state == GameState.GameResult)
            {
                break;
            }
        }
        action?.Invoke();
    }


}