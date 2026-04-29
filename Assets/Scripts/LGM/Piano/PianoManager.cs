using Bax.P0.Client.UnityWorld.PictureGame;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using static Settings;

public class PianoManager : Singleton<PianoManager>
{
    public float minSize = 0.1f;
    public float maxSize = 1;
    public Transform noteParent;
    public GameObject[] notePrefab;

    public EnumClass stateClass;
    public GameUI gameUI;
    public ScreenProsess screenProsess;
    public ZoZoBasePatton<PianoManager> zozo;

    private void Awake()
    {
        stateClass = new EnumClass();

        #region 공용 스테이트 패턴 

        ActionProcess.Enter_StateListener(null , null, null, null);

        zozo = new ZoZoBasePatton<PianoManager>();
        zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
        #endregion
    }
    // 건반 이벤트
    public void ButtonEvent(Transform _tranform)
    {
        // _tranform 위치에 음표 생성 후 랜덤한 크기로 설정
        GameObject obj = CreateNode(_tranform.position);
        obj.transform.localScale = RandomSize(minSize, maxSize);
    }
    // 랜덤 음표 생성
    public GameObject CreateNode(Vector3 _pos)
    {
        // 랜덤한 음표 생성 후 반환
        int index = Random.Range(0, notePrefab.Length);
        return Instantiate(notePrefab[index], _pos, Quaternion.identity, noteParent);
    }
    // 일정한 가로 세로의 랜덤 사이즈 반환
    public Vector3 RandomSize(float min, float max)
    {
        // min ~ max까지의 랜덤한 크기 반환
        float scale = Random.Range(min, max);
        return new Vector3(scale, scale, 0);
    }
}
