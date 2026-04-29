using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchMotionScoreController : MonoBehaviour
{
    public string fileName;
    public UIAtlas numericsAtlas;
    public UISprite[] numericArr;
    public UISprite imgScore; //regist image Score from imgAtlas
    [Range(0.1f, 1f)]
    public float aniTime;
    [Range(20f, 50f)]
    public float jumpHeight;
    [Range(0.1f, 10f)]
    public float aniSpeed;
    public bool bFixedText;
    public bool bAnimation;

    public bool Active
    {
        get
        {
            if (this == null) return false;
            return gameObject.activeInHierarchy;
        }
        set
        {
            if (this != null)
                gameObject.SetActive(value);
        }
    }

    public int Score
    {
        get { return _score; }
        set
        {
            if (_score != value)
            {
                int prevScore = _score;
                _score = value;
                _ChangeNumerics(prevScore, _score);
            }
        }
    }

    private int _score;
    private Vector3 _numeric0Pos;
    private Coroutine _numericAnimation;

    void Awake()
    {
        _numeric0Pos = numericArr[0].cachedTransform.localPosition;
        for (int i = 1; i < numericArr.Length; ++i)
            numericArr[i].cachedGameObject.SetActive(false);
        if (aniTime == 0) aniTime = 0.5f;
        if (jumpHeight == 0) jumpHeight = 30f;
        if (aniSpeed == 0) aniSpeed = 5f;
    }

    void OnEnable()
    {
        Score = 0;
    }

    public static void ChangScore(UISprite[] _images , List<int>_score, string _fileName)
    {
        int _constraintLength = _score.Count;

        for (int i = 0, len = _images.Length; i < len; ++i)
        {
            if(i < _constraintLength)
            {
                if (!_images[i].cachedGameObject.activeInHierarchy )
                    _images[i].cachedGameObject.SetActive(true);
                _images[i].spriteName = _fileName + _score[i].ToString();
                
                float _x = _images[i].width * 0.75f * ((_constraintLength * 0.5f - 0.5f) - (1f * i));

                _images[i].cachedTransform.localPosition = new Vector3(_x, 0f, 0f);
            }
            else
                _images[i].cachedGameObject.SetActive(false);
        }
    }

    private void _ChangeNumerics(int prevScore, int newScore)
    {
        // 점수에 맞게 Sprite를 표시한다.
        List<int> numbers = NumericSplit.Split(newScore);
        for (int i = 0, len = numericArr.Length; i < len; i++)
        {
            if (i < numbers.Count)
            {
                numericArr[i].cachedGameObject.SetActive(true);
                numericArr[i].spriteName = string.Format("{0}{1}", fileName, numbers[i]);
            }
            else
            {
                numericArr[i].cachedGameObject.SetActive(false);
            }
        }

        // Sprite Y 위치 초기화
        if (_numericAnimation != null) StopCoroutine(_numericAnimation);
        for (int i = 0; i < numericArr.Length; i++)
        {
            Vector3 pos = numericArr[i].cachedTransform.localPosition;
            pos.y = _numeric0Pos.y;
            numericArr[i].cachedTransform.localPosition = pos;
        }

        if (bAnimation && Active && newScore > 0)
        {
            // 애니메이션이 필요할 경우 실행
            List<int> prevNumbers = NumericSplit.Split(prevScore);
            _numericAnimation = StartCoroutine(NumericAniProcess(prevNumbers, numbers));
        }

        // 가변 폰트일 때 배경 위치 조정
        if (imgScore != null && numbers.Count < numericArr.Length && !bFixedText)
        {
            Vector3 pos = numericArr[numbers.Count].cachedTransform.localPosition;
            pos.x -= numericArr[numbers.Count].width;
            imgScore.cachedTransform.localPosition = pos;
        }
    }

    IEnumerator NumericAniProcess(List<int> prevNubmers, List<int> newNumbers)
    {
        int fromIndex = 0;
        int toIndex = 0;
        if (prevNubmers.Count != newNumbers.Count)
            toIndex = Mathf.Min(numericArr.Length, newNumbers.Count);
        else
        {
            for (int i = 0; i < newNumbers.Count; i++)
            {
                if (prevNubmers[i] != newNumbers[i])
                    toIndex = Mathf.Min(numericArr.Length, i + 1);
            }
        }
        
        float fTime = 0;
        for(int i = fromIndex; i < toIndex; i++)
        {
            Vector3 pos = numericArr[i].cachedTransform.localPosition;
            numericArr[i].cachedTransform.localPosition = new Vector3(pos.x, pos.y + jumpHeight, pos.z);
        }
        float distance = jumpHeight;
        yield return null;

        do
        {
            float curDistance = distance / aniTime;
            fTime += Time.deltaTime;
            curDistance += fTime * aniSpeed;
            Vector3 temp = Vector3.up * curDistance;

            if (distance - curDistance > 0)
            { distance -= curDistance; }
            else { distance = 0; }

            for (int i = fromIndex; i < toIndex; i++)
                numericArr[i].cachedTransform.localPosition -= temp;
            yield return null;
        }
        while (fTime < aniTime && distance > 0);

        for (int i = fromIndex; i < toIndex; i++)
        {
            Vector3 pos = numericArr[i].cachedTransform.localPosition;
            numericArr[i].cachedTransform.localPosition = new Vector3(pos.x, _numeric0Pos.y, pos.z);
        }
    }
}

