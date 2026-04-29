using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 드래곤이 출 춤 목록을 저장하는 클래스. 춤 동작 제어는 DragonMotionBehaviour 클래스에서 담당.
/// </summary>
[RequireComponent(typeof(Animator))]
public class DragonMotionList : MonoBehaviour
{
    public List<int> motions = new List<int>();
    public int index = -1;

    public System.Action onFinished;

    private Animator _animator;

    public void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Set(List<int> newMotions = null)
    {
        motions.Clear();
        if (newMotions != null)
            motions.AddRange(newMotions);
        index = -1;
    }

    public void Play()
    {
        _animator.SetInteger("motion_num", 0);
        _animator.SetTrigger("motion");
    }

    public void Stop()
    {
        index = motions.Count;
        _animator.SetInteger("motion_num", 0);
        _animator.SetTrigger("motion");
    }
}
