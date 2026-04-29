using UnityEngine;
using System.Collections;

public class UIResult : MonoBehaviour 
{
	public GameObject objCongratulations;
	public GameObject objGameOver;
    public GameObject objResult;
	public AnimationCurve aniCurve;
	public Transform oriPos;
	public Transform targetPos;
	public EventDelegate events;


	void OnEnable()
	{
		Init();
	}

	void OnDisable()
	{
		Init();
	}

	public void Init()
	{
        if(objCongratulations)
		    objCongratulations.SetActive(false);
        if(objGameOver)
		    objGameOver.SetActive(false);
        if(objResult)
            objResult.SetActive(false);
	}

	public void Play(GameObject obj)
	{
		if(!obj.activeInHierarchy) obj.SetActive(true);
		TweenTransform tween = TweenTransform.Begin(obj , 0.5f, oriPos, targetPos);
		tween.animationCurve = aniCurve;

        if (events != null)
        {
            if (!tween.onFinished.Contains(events))
            { tween.onFinished.Add(events); }
        }
	}
    public void RemoveEvent()
    {
        events = null;
    }
}
