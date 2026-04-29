using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChangeController : MonoBehaviour
{
    public void NextContent(int _nextMode)
    {
        BigboardContentMode mode = (BigboardContentMode)_nextMode;
        StartCoroutine(SceneChangeProcess(mode.ToString()));
    }
    
    IEnumerator SceneChangeProcess(string _nextMode)
    {
        yield return new WaitForEndOfFrame();

        SceneManager.LoadSceneAsync(_nextMode);

        /*
        var async = SceneManager.LoadSceneAsync(_nextMode);
        float waitTime = 1.0f;
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            yield return null;
            waitTime -= Time.deltaTime;
        }

        if (waitTime > 0.0f)
            yield return new WaitForSeconds(waitTime);

        async.allowSceneActivation = true;
        */
    }
}