using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DoTweenMove : MonoBehaviour {
    public Transform target;

    public float duration;
    public bool autoStart;

    Transform cachedTransform;
	
    void Awake()
    {
        cachedTransform = this.transform;
    }

	void Start () {
        if (autoStart)
        { DOPath(); }
        //targetTransform.DOPath(iTweenPath.GetPath(pathName), speed, PathType.CatmullRom, PathMode.Full3D, 10).SetSpeedBased(true).SetId(GetInstanceID()).OnComplete(OnMovePathFinish);//reference DragonPath.cs
	}

    public void DOPath(TweenCallback _callback)
    {
        DG.Tweening.Core.TweenerCore<Vector3, DG.Tweening.Plugins.Core.PathCore.Path, DG.Tweening.Plugins.Options.PathOptions> temp = this.transform.DOPath(iTweenPath.GetPath("alphabetpath"), duration, PathType.CatmullRom, PathMode.Full3D, 10).SetSpeedBased(true);//.OnComplete(); 

        if(_callback != null)
        {   temp.SetId(GetInstanceID()).OnComplete(_callback);  }
    }

    public void DOPath()
    {
        DG.Tweening.Core.TweenerCore<Vector3, DG.Tweening.Plugins.Core.PathCore.Path, DG.Tweening.Plugins.Options.PathOptions> temp = this.transform.DOPath(iTweenPath.GetPath("alphabetpath"), duration, PathType.CatmullRom, PathMode.Full3D, 10).SetSpeedBased(true);//.OnComplete(); 
        temp.SetId(GetInstanceID()).OnComplete(() =>
        {
            this.DOPath();
        }

            );
        
    }



    void Update()
    {
        if (target != null)
        {
            cachedTransform.LookAt(target);
        }
    }
}
