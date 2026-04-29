using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchEvent : MonoBehaviour
{
    // 싱글톤
    static TouchEvent _instant;

    // 외부 수정 불가, 읽기만 가능 하도록
    public static TouchEvent Instance { get => _instant; }

    private int layerMask_Monster = 1 << 3; // Layer 3 == Monster
    private int layerMask_BackMonster = 1 << 6; // Layer 6 == BackMonster

    // 레이 최대 거리
    private float maxDistance = 200000.0f;

    [SerializeField]
    GameObject badImage;

    [SerializeField]
    GameObject goodImage;

    [SerializeField]
    Canvas canvas;

    private void Awake()
    {
        if (!_instant)
        {
            _instant = this;
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 생성 된 오브젝트 눌렀을때
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask_Monster))
            {
                hit.collider.GetComponent<DinoNavMesh>().OnTouched();

                print("눌렀다3" + hit.transform.gameObject);
            }

            // 동물테마 - 백그라운드 동물 눌렀을때
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask_BackMonster))
            {
                hit.collider.GetComponent<BackAnimalsTouch>().BackMonsterTouch();

                print("동물터치");
            }

        }
    }

    // 버튼 잘못 클릭했을때
    public void badView(Transform tr)
    {
        badImage.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);
        GameObject bad_img = Instantiate(badImage);
        // click 된 버튼에 보여주기
        bad_img.transform.parent = tr.parent;
        bad_img.transform.localRotation = Quaternion.identity;
        bad_img.transform.position = tr.position;
        Destroy(bad_img, 0.2f);
    }

    // 버튼 제대로 클릭했을때
    public void goodView(Transform tr)
    {
        goodImage.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);
        GameObject good_img = Instantiate(goodImage);
        // click 된 버튼에 보여주기
        good_img.transform.parent = tr.parent;
        good_img.transform.localRotation = Quaternion.identity;
        good_img.transform.position = tr.position;
        Destroy(good_img, 0.2f);
    }
}

