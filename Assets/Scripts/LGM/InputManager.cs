using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class RayObjInfo2D
{
    public GameObject obj;
    public Vector3 point = Vector3.one;
}
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;    // InputManager 싱글톤
    public float maxDis;    // Ray사거리

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (Input.GetMouseButtonDown(0) &&
            RayCastObj(out GameObject go,
            LayerMask.NameToLayer("Plane"))) 
        {
            Debug.Log(go);
        }
    }

    // 충돌 오브젝트 반환
    public bool RayCastObj(out GameObject go)
    {
        // 마우스 좌표를 스크린 Ray 좌표로 변경
        Ray ray = Camera.main.ScreenPointToRay(Settings.instance.MousePos());
        RaycastHit hit;

        // 충돌한 오브젝트 정보 및 true 반환
        if (Physics.Raycast(ray, out hit, maxDis))
        {
            go = hit.transform.gameObject;
            return true;
        }
        go = null;
        return false;
    }
    // 충돌한 오브젝트 layer비교 후 오브젝트 반환
    public bool RayCastObj(out GameObject go, int layer)
    {
        // 마우스 좌표를 스크린 Ray 좌표로 변경
        Ray ray = Camera.main.ScreenPointToRay(Settings.instance.MousePos());
        RaycastHit hit;
        // 충돌한 layer 오브젝트 정보 및 true 반환
        if (Physics.Raycast(ray, out hit, maxDis, 1 << layer)) 
        {
            go = hit.transform.gameObject;
            return true;
        }
        
        go = null;
        return false;
    }

    // 충돌한 2D 오브젝트 layer비교 후 오브젝트 반환
    public bool RayCastObj2D(RayObjInfo2D info, int layer)
    {
        // 레이저를 발사할 좌표
        Vector2 startPoint = Camera.main.ScreenToWorldPoint(
            Settings.instance.MousePos());
        // startPoint에서 forward 방향으로 레이저 발사
        Ray2D ray = new Ray2D(startPoint, Vector3.forward);
        // 충돌한 2D 오브젝트 정보를 hit에 저장
        RaycastHit2D hit = Physics2D.Raycast(
            ray.origin, ray.direction, maxDis, 1 << layer);
        
        // hit가 null이 아니면 충돌한 오브젝트 정보를 보내고 true 반환
        if (hit)
        {
            info.obj = hit.transform.gameObject;
            info.point = hit.point;
            return true;
        }

        info = null;
        return false;
    }
}
