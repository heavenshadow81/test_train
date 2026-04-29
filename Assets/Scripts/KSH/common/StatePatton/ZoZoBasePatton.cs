using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ZoZoBase<T> where T : MonoBehaviour
{
    public ZoZoBase() { }

    public virtual void Enter(Action action) { }

    public virtual void Excute(Action action) { }

    public virtual void Exit(Action action) { }

}

public class ZoZoIntro<T> : ZoZoBase<T> where T : MonoBehaviour
{

    public ZoZoIntro() { }

    public override void Enter(Action action)
    {
       action?.Invoke();

        Debug.Log("Intro");
    }
}

public class ZoZoWait<T> : ZoZoBase<T> where T : MonoBehaviour
{
    public ZoZoWait() { }

    public override void Enter(Action action)
    {
        action?.Invoke();
        Debug.Log("Wait");
    }
}


public class ZoZoPlay<T> : ZoZoBase<T> where T : MonoBehaviour
{
    public ZoZoPlay() { }

    public override void Enter(Action action)
    {
        action?.Invoke();
        Debug.Log("Game");
    }

    public override void Excute(Action action)
    {
        action?.Invoke();
        Debug.Log("Game Update");
    }


}

public class ZoZoResult<T> : ZoZoBase<T> where T : MonoBehaviour
{
    public ZoZoResult() { }
    public override void Enter(Action action)
    {
        action?.Invoke();
        ActionProcess.Result?.Invoke();
        Debug.Log("Result");
    }
}

/*[System.Serializable]*/
public class ZoZoBasePatton<T> where T : MonoBehaviour
{
    public ZoZoBase<T> MGR; // 매니저
    public ReadyProcess readyProcess;   // Init(초기화)일떄 실행
    public ResultProcess resultProcess; // 종료될때 실행
    public ScreenProsess screenProsess; // ui관련 구현 부분 실행

    public EnumClass stateClass;

   

    public void Init(EnumClass stateClass,ScreenProsess screen ,ReadyProcess ready , ResultProcess result)
    {
        this.stateClass = stateClass;

        screenProsess = screen;
        readyProcess = ready;
        resultProcess = result;
        Change(GameState.GameIntro);
    }
    public void Change(GameState state)
    {
        if (MGR != null) MGR.Exit(null);

        MGR = instanceBase(state);

        MGR.Enter(Enter);
    }
    public void Enter()
    {
        readyProcess = new ReadyProcess(screenProsess);

        switch (stateClass.state)
        {
            case GameState.GameIntro:
                ActionProcess.Intro?.Invoke();
                readyProcess.GameRule(Camera.main, () => { Change(GameState.GameWait); });
                break;

            case GameState.GameWait:
                ActionProcess.Wait?.Invoke();
                //버튼에 연결  - 버튼 클릭시 fadeout 되고 게임플레이상태로 이동
                readyProcess.Ready(() =>
                {
                    readyProcess.PanelBackToAlphaZero();
                    Change(GameState.GamePlay);
                }).Forget();
                break;

            case GameState.GamePlay:
                ActionProcess.Play?.Invoke();
                break;

            case GameState.GameResult:
                
                ResultProcess resultProcess = new ResultProcess(screenProsess);
                resultProcess.Init(stateClass);
                break;
        }
    }

    private ZoZoBase<T> instanceBase(GameState state)
    {
        stateClass.state = state;
        switch (state)
        {
            case GameState.GameIntro: return new ZoZoIntro<T>();
            case GameState.GameWait: return new ZoZoWait<T>();
            case GameState.GamePlay: return new ZoZoPlay<T>();
            case GameState.GameResult: return new ZoZoResult<T>();
        }
        return null;
    }
}

