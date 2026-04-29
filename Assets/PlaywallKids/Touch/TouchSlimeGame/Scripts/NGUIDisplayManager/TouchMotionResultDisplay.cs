using UnityEngine;
using System.Collections;

public class TouchMotionResultDisplay : ResultObject
{
    public GameObject [] gameObjects;
    public string[] strings;
    [HideInInspector]
    public int [] points;

    void Awake()
    {
        if (points ==null)
            points = new int[]{0, 0, 0, 0, 0, 0};
    }

	public override void OnEnable()
    {
        base.OnEnable();
        for(int i = 0 ; i < gameObjects.Length ; ++i)
        {
            gameObjects[i].SetActive(false);
        }
    }

    public override void Display()
    {
        if (doDisplayig) return;
        doDisplayig = true;
        StartCoroutine(DisplayProcess());
    }

    IEnumerator DisplayProcess()
    {
        int _index = 0;

        while (_index < labels.Length)
        {
            yield return new WaitForSeconds(1f);
            int _pre = _index - 1;
            if (_pre >= 0 && _pre < gameObjects.Length - 1)
                gameObjects[_pre].SetActive(false);
            if(_index < gameObjects.Length)
                gameObjects[_index].SetActive(true);
            string _s = _index < strings.Length ? strings[_index] : "";
            labels[_index].text = string.Format("{0}{1}",_s, points[_index]);
            labels[_index].cachedGameObject.SetActive(true);
            ++_index;
        }
        yield return new WaitForSeconds(2f);
        doDisplayig = false;
    }
}
