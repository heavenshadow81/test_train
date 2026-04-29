using UnityEngine;
using System.Collections;

public class ProgramQuit : MonoBehaviour
{
    public UISprite fadeOutSprite;

    public void Quit()
    {
        StartCoroutine(QuitProcess(2f));
    }

    IEnumerator QuitProcess()
    {
        while (Application.isPlaying)
        {
            Application.Quit();
            yield return null;
        }
    }

    private IEnumerator QuitProcess(float time)
    {
        // Disable all children's colliders to prevent touch event.
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
            c.enabled = false;

        // Wait for seconds
        float t = 0.0f;
        fadeOutSprite.gameObject.SetActive(true);

        while (t < time)
        {
            t += Time.deltaTime;

            fadeOutSprite.alpha = t / time;
            yield return null;
        }
        fadeOutSprite.alpha = 1.0f;
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}