using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ExtensionMethod
{
    // 오브젝트 이미지 변경
    public static void ChangeSprite(this SpriteRenderer _sprite, Sprite changeSprite)
    {
        _sprite.sprite = changeSprite;
    }
    // 오브젝트 이미지 랜덤 변경 (sprites 배열 안의 항목 중에서 변경)
    public static void RandomChange(this SpriteRenderer _sprite, 
        Sprite[] sprites, int correction = 0)
    {
        int index = UnityEngine.Random.Range(0, sprites.Length + correction);
        _sprite.sprite = sprites[index >= sprites.Length ? 0 : index];
    }
    // obj의 SpriteRenderer 컴포넌트 반환
    public static SpriteRenderer GetSpriteRenderer(this GameObject obj)
    {
        if(obj.TryGetComponent(out SpriteRenderer _sprite))
        {
            return _sprite;
        }
        return null;
    }
    public static Vector2Int GetID(this GameObject obj)
    {
        if (obj.TryGetComponent(out SQPlane plane))
        {
            return plane.id;
        }
        return Vector2Int.zero;
    }
    public static void SetTrap(this GameObject obj, bool trap)
    {
        if (obj.TryGetComponent(out PlaneController plane))
        {
            plane.isTrap = trap;
            return;
        }
    }
    public static Vector3 RandomPos(this Vector3 target, float dis)
    {
        // target에서 dis거리 이내의 랜덤한 위치
        float _x = UnityEngine.Random.Range(-target.x - dis*2, target.x + dis*2);
        float _y = UnityEngine.Random.Range(-target.y - dis, target.y + dis);
        return new Vector3(_x, _y);
    }

    // 활성화 갯수 반환
    public static int ACount(this List<GameObject> obj)
    {
        int count = 0;
        foreach(var a in obj)
        {
            // 활성화된 오브젝트의 갯수 기록
            if (a.activeSelf) count++;
        }
        return count;
    }

    public static void SetCheildObj(this List<GameObject> obj, Transform parent)
    {
        for(int i = 0; i < parent.childCount; i++)
        {
            obj.Add(parent.GetChild(i).gameObject);
        }
    }
    
    public static Vector2 Sum(this Vector2 a, Vector2 b)
    {
        a.x += b.x;
        a.y += b.y;
        return a;
    }
    public static Vector3 Sum(this Vector3 a, Vector3 b)
    {
        a.x += b.x;
        a.y += b.y;
        a.z += b.z;
        return a;
    }
    public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
    {
        if (value.x < min.x)
        {
            value.x = min.x;
        }
        if (value.y < min.y)
        {
            value.y = min.y;
        }
        if (value.z < min.z)
        {
            value.z = min.z;
        }

        if (value.x > max.x)
        {
            value.x = max.x;
        }
        if (value.y > max.y)
        {
            value.y = max.y;
        }
        if (value.z > max.z)
        {
            value.z = max.z;
        }

        return value;
    }
    public static int GetSpriteOrderLayer(this GameObject _obj)
    {
        if(_obj.TryGetComponent(out SpriteRenderer _sprite))
        {
            return _sprite.sortingOrder;
        }
        return -1;
    }
    public static bool SetSpriteOrderLayer(this GameObject _obj, int _layer)
    {
        if (_obj.TryGetComponent(out SpriteRenderer _sprite))
        {
            _sprite.sortingOrder = _layer;
            return true;
        }
        return false;
    }
    public static GameObject Parent(this GameObject _obj)
    {
        return _obj.transform.parent.gameObject;
    }
    public static void SetImage(this GameObject obj, Sprite sprite)
    {
        if (obj.TryGetComponent(out Image _image))
        {
            _image.sprite = sprite;
            return;
        }
        Debug.Log("에러");
        return;
    }

    // 중복되지 않는 list내의 값을 count갯수 만큼 반환
    public static List<T> GetRandomNotOverlap<T>(this List<T> list, int count)
    {
        List<int> tempList = new List<int>();
        for (int i = 0; i < list.Count; i++)
            tempList.Add(i);

        for (int i = 0; i < list.Count; i++)
        {
            // 랜덤한 인덱스의 값과 스왑
            int a = UnityEngine.Random.Range(0, list.Count);
            int temp = tempList[i];
            tempList[i] = tempList[a];
            tempList[a] = temp;
        }

        // 랜덤하게 섞인 list의 0 ~ count까지의 값을 인덱스로 받아 중복되지 않은 랜덤 값 추출
        List<T> select = new List<T>();
        for (int i = 0; i < count; i++)
        {
            select.Add(list[tempList[i]]);
        }

        return select;
    }
    private static void screenProsess_completed(this Settings setting, AsyncOperation obj)
    {
        Settings.instance.ContantSettingPanelPos(Settings.ScreenRotation.Width);
    }

    // 카메라의 x,y 사이즈 조절
    public static Vector2 Size(this Camera cam)
    {
        float y = cam.orthographicSize; // 카메라 OrthoGraphic 상테일때 카메라 수평 크기
        float x = y * cam.aspect;   // 수평 * 카메라 비율
        return new Vector2(x, y);
    }
}
