using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ActionProcess
{
    public static Action SandDeletes = () => { };
    public static Action SandAllRaise = () => { };


    //상태 시작할때 실행되는 이벤트 Action
    public static Action Intro = () => { };
    public static Action Wait = () => { };
    public static Action Play = () => { };
    public static Action Result = () => { };


    public static Action ActiveSceneEvent = () => { };


    public static Action Intro_rectMenuPosition = () => { };

    public static Func<string> GetSceneName;

    /// <summary>
    /// 스테이트 시작시 실행되는 기능 추가
    /// //null일 경우 실행안됨
    /// </summary>
    /// <param name="intro"></param>
    /// <param name="wait"></param>
    /// <param name="play"></param>
    /// <param name="result"></param>
    public static void Enter_StateListener(Action intro , Action wait , Action play , Action result)
    { 
        Intro = intro;
        Wait = wait;
        Play = play;
        Result = result;
    }
}
