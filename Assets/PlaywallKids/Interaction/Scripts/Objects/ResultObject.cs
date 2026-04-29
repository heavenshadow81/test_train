using UnityEngine;
using System.Collections;

public class ResultObject : MonoBehaviour
{
    public UILabel[] labels;
    protected bool doDisplayig;

    public new bool active
    {
        set
        {
            this.gameObject.SetActive(value);
        }

        get
        {
            return this.gameObject.activeInHierarchy;
        }
    }

    public string[] texts
    {
        set
        {
            if (value != null && value.Length > 0)
            {
                string[] _string = value;
                for (int i = 0, len = labels.Length; i < _string.Length && i < len; ++i)
                {
                    labels[i].text = _string[i];
                }
            }
        }
    }

    public virtual void OnEnable()
    {
        for (int i = 0, len = labels.Length; i < len; ++i)
        {
            labels[i].cachedGameObject.SetActive(false);
        }
    }

    public virtual void Display()
    {
        if (!doDisplayig)
        {
            doDisplayig = true;
            TweenScale _tw = TweenScale.Begin(this.gameObject, 1f, Vector3.one);
            _tw.from = Vector3.zero;
            _tw.onFinished.Add((new EventDelegate(() => { StartCoroutine(DisplayProcess()); })));
            _tw.method = UITweener.Method.BounceOut;
        }
    }

    IEnumerator DisplayProcess()
    {
        Transform cachedTransform = this.transform;

        for (int i = 0, len = labels.Length; i < len; ++i)
        {
            labels[i].cachedGameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
        doDisplayig = false;
    }
}