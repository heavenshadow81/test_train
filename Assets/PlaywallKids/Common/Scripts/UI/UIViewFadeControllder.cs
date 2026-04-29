using UnityEngine;
using System.Collections;

public class UIViewFadeControllder : MonoBehaviour
{
    [Range(0.01f, 5f)]
    public float speed;

    public float alpha
    {
        get { return Curtain.alpha; }
        set { Curtain.alpha = value; }
    }

    private UISprite Curtain
    {
        get
        {
            if (_curtain == null)
                _curtain = GetComponentInChildren<UISprite>(true);
            return _curtain;
        }
    }

    bool bActive;
    UISprite _curtain;
    Coroutine _coroutine;

    public bool Active
    {
        get
        {
            return gameObject.activeInHierarchy;
        }

        set
        {
            gameObject.SetActive(value);
        }
    }

    void OnEnable()
    {
        if (speed == 0) speed = 1f;
        bActive = false;
    }

    void OnDisable()
    {
        if (null != _coroutine)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    public void FadeEffect(bool _ascending, float _targetValue, EventDelegate _callBackFunc = null)
    {
        if (bActive || !gameObject.activeInHierarchy) return;
        bActive = true;
        _coroutine = StartCoroutine(FadeEffectProcess(_ascending, _targetValue, _callBackFunc));
    }

    IEnumerator FadeEffectProcess(bool _ascending, float _targetValue, EventDelegate _callBackFunc)
    {
        bool _bComplete = false;
        float _value = 0;
        do
        {
            _value = Curtain.alpha;
            _value += Time.deltaTime * speed * (_ascending ? 1f : -1f);
            _value = (_ascending && (_value >= _targetValue)) || (!_ascending && (_value <= _targetValue)) ? _targetValue : _value;
            Curtain.alpha = _value;
            _bComplete = (_value == _targetValue);
            yield return new WaitForEndOfFrame();
        } while (!_bComplete);

        if (_callBackFunc != null) _callBackFunc.Execute();
        bActive = false;
    }
}