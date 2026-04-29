using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.MapoContents.WorldTravel;

public class LoadingCircle : MonoBehaviour {
    Image _inCircle;
    Image _outCircle;
    Text _percentText;
    float _percent;
    bool _isLoading;
    float speed = 0;
    void Awake ()
    {
        _outCircle = this.GetComponent<Image>();
        _inCircle = this.transform.GetChild(0).GetComponent<Image>();
        _percentText = this.transform.GetChild(1).GetComponent<Text>();
        Init();
    }

    void Init()
    {
        Color tmp;
        //로딩텍스트 초기화
        _percentText.text = "";
        //로딩서클 초기화
        _inCircle.fillAmount = 0;
        //외각서클 알파값 비활성화
        tmp = _outCircle.color;
        tmp.a = 0;
        _outCircle.color = tmp;
        _percent = 0;
        _isLoading = false;
    }

    public void StartLoading()
    {
        //코루틴 중복입장을 막기위한 예외처리
        if(!_isLoading)
            StartCoroutine(LoadingProcess());
    }

    IEnumerator LoadingProcess()
    {
        _isLoading = true;
        
        //로딩서클 0%로 초기화
        _inCircle.fillAmount = 0;

        //외각서클 알파값 활성화
        Color tmp = _outCircle.color;
        tmp.a = 1;
        _outCircle.color = tmp;

        //로딩 시작
        while (true)
        {
            if (_inCircle.fillAmount >= 0.99f)
            {
                _inCircle.fillAmount = 0.99f;
                _percentText.text = "99";
                break;
            }
            else
            {
                _percent += speed;
                if (_percent >= 0.99f)
                    _percent = 0.99f;
                _inCircle.fillAmount = _percent;
                _percentText.text = (_percent * 100).ToString("N0");
                speed += Random.Range(0.001f, 0.01f);
            }
            yield return new WaitForSeconds(0.1f);
        }
        //99에서 로딩 종료 후 대기
        yield return new WaitForSeconds(0.5f);

        _percentText.text = "100";
        _inCircle.fillAmount = 1;
        yield return new WaitForSeconds(0.5f);

        Init();
        TravelManager.Instance.CompleteLoading();
        yield return null;
    }
}
