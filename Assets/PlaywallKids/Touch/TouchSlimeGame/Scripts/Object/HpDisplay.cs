using UnityEngine;
using System.Collections;

public class HpDisplay : MonoBehaviour {
    [Range(0.1f, 1f)]
    public float hitActiveTime;
    [Range(0.1f, 10f)]
    public float speed;

    public Transform cachedTransform
    {  get {return hpBar.cachedTransform;} }

    UISlider hpBar;
    Color color;
    float max_width;
    float fTime;
    bool bUnderAttack;
    bool bOnOff;
    
    void Awake()
    {
        if (hitActiveTime == 0) hitActiveTime = 0.5f;
        if (speed == 0) speed = 5f;
        hpBar = GetComponent<UISlider>();
        max_width = hpBar.foregroundWidget.cachedTransform.localScale.x;
        color = hpBar.foregroundWidget.color;
    }

    void OnEnable()
    {
        bUnderAttack = false;
        cachedTransform.localScale = Vector3.one;
        hpBar.foregroundWidget.cachedTransform.localScale = new Vector3(max_width, cachedTransform.localScale.y, 1);
    }

    public void UpdateDisplay(float _percent)
    {
        if (_percent * max_width < 0) return;
        hpBar.value = _percent;
    }

    void Update()
    {
        if (hpBar.value < 0.4f)
        {
            fTime++;
            if (fTime % 4 == 0)
            {
                fTime = 0;
                hpBar.foregroundWidget.color = bOnOff ? Color.yellow : color;
                bOnOff = !bOnOff;
            }
        }
    }

    public void Hit()
    {
        if(!bUnderAttack)
        {
            bUnderAttack = true;
             StartCoroutine(HitProcess());
        }
    }

    IEnumerator HitProcess()
    {
        float time = 0;
        do
        {
            time += Time.deltaTime;
            int n = (time < hitActiveTime / 2) ? -1 : 1;
            cachedTransform.localScale += n * new Vector3(Time.deltaTime * speed, Time.deltaTime * speed, 1);
            yield return new WaitForEndOfFrame();
        } while (time < hitActiveTime);
        
        cachedTransform.localScale = Vector3.one;
        bUnderAttack = false;
    }
}
