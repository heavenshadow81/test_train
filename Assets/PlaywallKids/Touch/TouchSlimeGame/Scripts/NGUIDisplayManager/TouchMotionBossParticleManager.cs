using UnityEngine;
using System.Collections;

public enum EParticleType
{
     NONE, HIT, ARRIVE, KO
}

public class TouchMotionBossParticleManager : MonoBehaviour {
    public GameObject hitParticle;
    public GameObject arriveParticle;
    public GameObject dieParticle;
    public Vector3 size;

    Transform mTransform;
    public Transform cachedTransform
    {
        get
        {
            if(mTransform == null)
            {
                mTransform = this.transform;
            }
            return mTransform;
        }
    }

    public void ParticleEmitt(EParticleType _type, Vector3 _pos)
    {
        GameObject temp = null;
        switch(_type)
        {
            case EParticleType.HIT:
                temp = hitParticle;
                break;
            case EParticleType.ARRIVE:
                temp = arriveParticle;
                break;
            case EParticleType.KO:
                temp = dieParticle;
                break;
        }

        temp = NGUITools.AddChild(cachedTransform.gameObject, temp);
        temp.transform.position = _pos;
        temp.transform.localScale = size;
        temp.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
