using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Loop : MonoBehaviour
{
    public AudioSource BGM_Source;
    public AudioClip[] BGMs;

    public int[] PlayTimes;
    public int timer;
    public int idx;
    private void Awake()
    {
        idx = 4;// Random.Range(0, BGMs.Length);
        BGM_Source.clip = BGMs[idx];
        BGM_Source.Play();
        StartCoroutine(BGMloop());
    }
    IEnumerator BGMloop()
    {
        while (true)
        {
            timer++;
            if (timer > PlayTimes[idx])
            {
                timer = 0;
                idx++;
                Debug.Log(idx + "/" + BGMs.Length);
                if (idx >= BGMs.Length)
                    idx = 0;
                Debug.Log("wait " + PlayTimes[idx]);
                BGM_Source.clip = BGMs[idx];
                BGM_Source.Play();
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
