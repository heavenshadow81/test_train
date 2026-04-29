using UnityEngine;
using System.Collections;

public class TouchMotionSmallObject : MonoBehaviour {
	#region Public variables

    const string HIT = "Hit";
    const string DIE = "Die";
    
	public int life = 1;
	public int _score = 0;
    public int Score
    {
        get
        {
            if (!this.gameObject.activeInHierarchy) gameObject.SetActive(true);
            StartCoroutine(DeadProcess());
            return _score;
        }
    }

	public float moveTime = 10.0f;
    public GameObject hitParticlePrefab;
	public GameObject dieParticlePrefab;
	public AudioClip hitSound;
	public AudioClip deadSound;
    [HideInInspector]
    public Camera cam;
    [HideInInspector]
    public Camera nguiCam;
    [HideInInspector]
    public bool bDestroy;
    [HideInInspector]
    public Renderer render;
	#endregion

    #region Private vlaue
    private float max_hp;
    private BezierMove bezier;
    private Transform camTransform
    {
        get
        {
            if (cam == null) cam = (GameObject.Find("GameCamera") as GameObject).GetComponent<Camera>();
            return cam.transform;
        }
    }

    GameObject particle;
    Collider mCol;
    public bool bCollider
    {
        set
        {
            if (mCol == null) mCol = GetComponent<Collider>();
            mCol.enabled = value;
        }

        get
        {
            return mCol.enabled;
        }
    }

    Animator mAni;
    Animator ani
    {
        get
        {
            if (mAni == null)
            {
                mAni = GetComponent<Animator>();
                if (mAni == null)
                   mAni = GetComponentInChildren<Animator>();
            }
            return mAni;
        }
    }

#endregion
	#region Unity Methods
	
    void Awake()
    {  
        max_hp = life;  
        render = this.gameObject.GetComponentInChildren<Renderer>();
        bezier = GetComponent<BezierMove>();
    }

    void FixedUpdate()
    {
        if (cam!= null)
        transform.LookAt(cam.transform);
    }

    void OnEnable()
    {
        life = (int)max_hp;
        bCollider = true;
        if (ani!=null)
            ani.SetBool(HIT, false);
        if (ani != null)
            ani.SetBool(DIE, false);
    }

    /// <summary>
    /// 피격 애니메이션
    /// </summary>
    /// <returns></returns>
    IEnumerator HitProcess()
    {
        DoHitting(true);
        yield return new WaitForSeconds(0.4f);
        DoHitting(false);
    }

    /// <summary>
    /// 히트 될 경우 뒤로 후퇴 및 피격 애니메이션
    /// </summary>
    /// <param name="_hit"></param>
    void DoHitting(bool _hit)
    {
        bCollider = !_hit;
        if (ani != null)
            ani.SetBool(HIT, _hit);
        bezier.fSpeed *= -1f;
    }

    /// <summary>
    /// 피격
    /// </summary>
    /// <returns></returns>
	public bool Hit()
    {
        if (!bCollider) return false;
		life -= 1;

		if(life > 0)
		{
            StartCoroutine(HitProcess());
            if (hitSound != null)
            {
                ML.PlaywallKids.Common.AudioPlay2D.PlayClip(hitSound);
               // AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position);
            }

            // 피격시 몸체 색상 변경
            if (render != null)
                render.materials[0].SetFloat("_Risks", 1.4f - (1.4f * ((float)life / max_hp)));
        }

        //피격 이펙트 생성
        if (hitParticlePrefab != null)
        {
            particle = (GameObject)Instantiate(hitParticlePrefab);
            particle.SetActive(true);

            particle.transform.parent = nguiCam.transform;
            //슬라임 월드 좌표 -> 뷰포트
            Vector3 pos = cam.WorldToViewportPoint(transform.position);
            //뷰포트 -> 스크린
            pos = cam.ViewportToScreenPoint(pos);
            //스크린 -> 배럴디스토션 스크린
            pos = cam.GetComponent<BarrelDistortionEffect>().GetOriginalScreenPosFromDistorted(pos);
            //베럴 디슽토션 스크린 좌표 -> 뷰포트
            pos = cam.ScreenToViewportPoint(pos);
            //뷰포트 -> NGUI 기준 월드 좌표
            pos = nguiCam.ViewportToWorldPoint(pos);
            pos.z = 0;
            particle.transform.position = pos;

            float dis = (cam.transform.position - pos).sqrMagnitude;
            /*
            particle.transform.parent = transform;
            particle.transform.position = transform.position;
            */

            particle.transform.rotation = Quaternion.Euler(Vector3.zero); // transform.rotation;
            particle.layer = nguiCam.gameObject.layer;

            /*
            ParticleSystem ps = particle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.startSize = size;// transform.localScale.x;
            }*/

            Destroy(particle, 6.0f);
        }
		
		return life < 1;
	}

	public void Die() 
    {
		Die (false);
	}

    /// <summary>
    /// 데드 이벤트
    /// </summary>
    /// <returns></returns>
    IEnumerator DeadProcess()
    {
        bCollider = false;
       
        if(ani != null)
        {
            ani.SetBool(HIT, false);
            ani.SetBool(DIE, true);
        }

        float bezierSpeed = bezier.fSpeed;
        bezier.fSpeed = 0f;
        if (deadSound != null)
            ML.PlaywallKids.Common.AudioPlay2D.PlayClip(deadSound);

        yield return new WaitForSeconds(0.7f);
        if (bDestroy)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
        ParticleEmitt(false);
        bezier.fSpeed = bezierSpeed;
    }

    /// <summary>
    /// 파티클 생성
    /// </summary>
    /// <param name="bDestroy"></param>
    void ParticleEmitt(bool bDestroy)
    {
        if (dieParticlePrefab != null)
        {
            particle = (GameObject)Instantiate(dieParticlePrefab);
            particle.SetActive(true);
            particle.transform.parent = cam.transform;
            particle.transform.position = transform.position;
            particle.transform.rotation = transform.rotation;
            particle.transform.localScale = Vector3.one;

            ParticleSystem ps = particle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var mainModule = ps.main;  // ParticleSystem의 main 모듈 가져오기
                mainModule.startSize = transform.localScale.x;  // startSize 설정
            }

            if (bDestroy) Destroy(particle, 5.0f);
        }
    }

    /// <summary>
    /// 죽음
    /// </summary>
    /// <param name="destroyImmediately"></param>
  	public void Die(bool destroyImmediately) {
		if (!destroyImmediately) 
        {
            ParticleEmitt(!destroyImmediately);
		}

        if (bDestroy)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
	}
	#endregion
}