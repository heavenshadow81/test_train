using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchMotionBossObject : MonoBehaviour {
    enum EState
    {  NONE, HIT, LEFT, RIGHT, IDLE, RUN, JUMP, READY, DIE, JUMPING_HIT, CLONE }

    enum EJumpType 
    { ARC, ZIGZAG, HOVER, STRAIGHT , NUM }
   
    public float minY, maxY, minX, maxX;
    public float idleTime;

    [HideInInspector]
    public Camera cam;
    private Transform camTransform
    {
        get
        {
            if (cam == null) cam = (GameObject.Find("GameCamera") as GameObject).GetComponent<Camera>();
            return cam.transform;
        }
    }

    #region Property

    HpDisplay mHp;
    public HpDisplay hpBar
    {
        get
        {
            if (mHp == null)
            { mHp = GameObject.FindObjectOfType<HpDisplay>(); }
            return mHp;
        }
    }

    private TouchMotionGameController _controller;
    public TouchMotionGameController controller
    {
        get
        {
            if (_controller == null)
            { _controller = GameObject.FindObjectOfType<TouchMotionGameController>(); }
            return _controller;
        }
    }

    public GameObject hitParticle; //CFX_MagicPoof
    public GameObject dieParticle; //CFX_SmokeExplosionAlt
    public GameObject arriveParticle; //CFX2_CartoonFight

    private GameObject _obj;
    public GameObject obj
    {
        get
        {
            if (_obj == null)
            { _obj = this.gameObject; }
            return _obj;
        }
    }

    private Transform _cachedTransform;
    public Transform cachedTransform
    {
        get
        {    if (_cachedTransform == null)
            { _cachedTransform = this.transform; }
            return _cachedTransform;  }
    }
    
    private Animator mAni;
    public Animator ani
    {
        get
        {
            if(mAni== null)
            {   mAni = obj.GetComponent<Animator>();  }
            return mAni;
        }
    }

    Collider mCollider;
    public bool bCollider
    {
        set
        {
            if (mCollider == null)
            {   mCollider = obj.GetComponent<Collider>(); }
            mCollider.enabled = value;
        }
    }


   static TouchMotionBossParticleManager mParticle;
   static TouchMotionBossParticleManager particle
    {
        get
        {
            if(mParticle == null)
            {
                mParticle = GameObject.FindObjectOfType<TouchMotionBossParticleManager>();
                Debug.Log(mParticle);
            }
            return mParticle;
        }
    }

    EState mState;
    EState currentState
    {
        set
        {
            if (mState != value || value == EState.NONE)
            {
                switch(value)
                {
                    case EState.JUMP:
                        if (mState != EState.JUMPING_HIT)
                        { StartCoroutine(JumpProcess()); }
                        break;

                    case EState.DIE :
                        StartCoroutine(DeadProcess());
                        break;

                    case EState.READY:
                        bCollider = true;
                        hpBar.gameObject.SetActive(true);
                        ani.SetBool("Idle", true);
                        ani.SetBool("Run", false);
                        postion = cachedTransform.localPosition;
                       
					    TweenPosition temp = obj.GetComponent<TweenPosition>();
					    if(temp != null) Destroy(temp);

                        break;
                    case EState.IDLE:
                        bCollider = true;
					    fActiveTime = 0;
                        ani.SetBool("Idle", true);
                        ani.SetBool("Right", false);
                        ani.SetBool("Left", false);
                        hpBar.gameObject.SetActive(true);
                        hpBar.UpdateDisplay((float)iLife / fMaxHp);
                        bezier.p0 = cachedTransform.localPosition;
                        bezier.p3 = cachedTransform.localPosition;
                        break;
                    case EState.HIT:
                        bCollider = false;
                        StartCoroutine( HitProcess() );
                        break;

                    case EState.LEFT:
					{
                        ani.SetBool("Idle", false);
                        ani.SetBool("Left", true);
                        Vector3 target = cachedTransform.localPosition;
						target.x -= jumpWidth;
                        target.y = postion.y;
                        target.z = postion.z;
                        StartCoroutine(BezierJumpProcess(target, iJumpCnt));
					}
                        break;
                    case EState.RIGHT: 
				    {
                        ani.SetBool("Idle", false);
                        ani.SetBool("Right", true);
                        Vector3 target = cachedTransform.localPosition;
					    target.x += jumpWidth;
                        target.y = postion.y;
                        target.z = postion.z;
                        StartCoroutine(BezierJumpProcess(target, iJumpCnt));
				    }
                        break;
                    case EState.RUN:
                        ani.SetTrigger("Run");
                        ani.SetBool("Idle", false);
                        break;

                    case EState.NONE: 
                        fActiveTime = 0;
                        bCollider   = false;
                        preDir      = EState.NONE;
                        if (hpBar!= null)
                        hpBar.gameObject.SetActive(false);
                        if (jumpWidth == 0)  jumpWidth = 1f;
                        if (jumpHeight == 0) jumpHeight = 1f;
                        if (jumpDept == 0)   jumpDept = 2f;
                        break;

                    case EState.CLONE://횬재 사용안함
                        bCollider = false;
                        iCntClone = iCntClone > 0 ? iCntClone - 1 : 0;
                        StartCoroutine(CloneProcess());
                        break;
                }
             }
            mState = value;
        }
        get
        { return mState; }
    }

    Vector3 mBezierHandle;
    Vector3 bezierHandle
    {
        get
        {
            switch (mJumptype)
            {
                case EJumpType.ARC:
                    mBezierHandle = new Vector3(0, jumpHeight, 0);
                    break;
                case EJumpType.HOVER:
                    mBezierHandle = new Vector3(0, jumpHeight, jumpDept);
                    break;
                case EJumpType.STRAIGHT:
                    mBezierHandle = Vector3.zero;
                    break;
                case EJumpType.ZIGZAG:
                    mBezierHandle = new Vector3(0, jumpHeight, 0);
                    break;
            }
            return mBezierHandle;
        }
    }

    Bezier mBezier;
    Bezier bezier
    {
        get
        {
            if(mBezier == null)
            {
                Vector3 p = new Vector3(0, 0.2f, 0);
                mBezier = new Bezier(cachedTransform.localPosition, p, -1 * p, cachedTransform.localPosition);
            }
            return mBezier;
        }
        set
        {   mBezier = value; }
    }

    Vector3 hpPostion
    {
        get
        {
             Vector3  pos =cachedTransform.position;
             pos.y += cachedTransform.localScale.y * 0.25f;
             pos.x -= cachedTransform.localScale.x * 0.05f;
             return controller.WorldToBarrelScreen(pos);
        }
    }

    Renderer mRender;
    Renderer render
    {
        get
        {
            if(mRender == null)
            {
                Renderer[] renders = obj.GetComponentsInChildren<Renderer>();
                for(int i = 0 ; i< renders.Length ; ++i)
                {
                    if (renders[i].tag == "Slime")
                    {
                        mRender = renders[i];
                        break;
                    }
                }
                if (mRender == null) mRender = obj.GetComponent<Renderer>();
            }
            return mRender;
        }
    }

    #endregion

	#region Range
	[Range(1, 400)]
    public int iLife;
	[Range(1f,10f)]
	public float jumpHeight;
	[Range(1f,10f)]
	public float jumpWidth;
    [Range(1, 10f)]
    public float jumpDept;
    [Range(1, 15)]
    public int max_jump = 6;
    [Range(0.01f, 1f)]
    public float jumpSpeed;
	#endregion

    public int iHitCnt;
    public AudioClip hitSnd;
    public AudioClip deadSnd;
    public AudioClip arriveSnd;

    #region Private
    EState preDir;
    EJumpType mJumptype;
    List<EJumpType> jumpList;
    Vector3 postion;
    float fActiveTime;
    float fMaxHp;
    int iJumpCnt;
    int bitLeft;
    int bitRight;
    bool bClone;
    int iCntClone;
    #endregion


    void Awake()
    {
        hitParticle.SetActive(false);
        dieParticle.SetActive(false);
        currentState = EState.NONE;
        jumpList = new List<EJumpType>();
        fMaxHp = iLife;
        
    }
    
    void OnEnable()
    {
        if (iLife == 0) iLife = 300; 
        if (idleTime == 0) idleTime = 6f;
        currentState = EState.NONE;
        currentState = EState.RUN;
        iCntClone = 5;
        bClone = false;
    }

    void OnDisable()
    {
        
    }

    void FixedUpdate()
    { 
        fActiveTime += Time.fixedDeltaTime;

        cachedTransform.LookAt(camTransform);
        hpBar.cachedTransform.position = hpPostion;
        switch(currentState)
        {
			case EState.READY:
				currentState = EState.IDLE;
				break;

	        case EState.IDLE:
	            fActiveTime += Time.fixedDeltaTime;
                if (fActiveTime >= idleTime)
                {   currentState = EState.JUMP;  }
                else
                {
                    float fIdleTime = fActiveTime % 1f;
                    cachedTransform.localPosition = bezier.GetPointAtTime(fIdleTime);
                }
	            break;
        }
    }

    #region Public Func
    /// <summary>
    /// 보스 슬라임이 움직일 영역 설정
    /// </summary>
    /// <param name="_minX"></param>
    /// <param name="_maxX"></param>
    /// <param name="_minY"></param>
    /// <param name="_maxY"></param>
    public void SetArea(float _minX, float _maxX, float _minY, float _maxY)
    {
        minX = _minX;
        maxX = _maxX;
        minY = _minY;
        maxY = _maxY;
    }

    public void Die()
    {   currentState = EState.DIE;  }

    public void Ready()
    { currentState = EState.READY; }

    public bool Hit()
    {
        if (iHitCnt < 5)
        {
            iHitCnt++;
            --iLife;
            if (iLife > 0)
            {
                if (currentState == EState.IDLE)
                { currentState = EState.HIT; }
                else if (currentState == EState.JUMP)
                { currentState = EState.JUMPING_HIT; }
/*
                if (((float)iLife / fMaxHp) <= iCntClone * 0.1)
                { currentState = EState.CLONE; }
 */ 
            }
            else
            {
                iLife = 0;
                Die();
                return true;
            }
        }
        else
        {
            currentState = EState.JUMP;
            iHitCnt = 0;
        }
        return false;
    }
    #endregion



    #region Private Func
    EJumpType GetJump(int i)
    { return (EJumpType)i; }

    /// <summary>
    /// 연속 점프
    /// </summary>
    /// <param name="_cnt"></param>
    /// <returns></returns>
    List<EJumpType> SetJumpList(int _cnt = 0)
    {
        if (_cnt == 0)
        { iJumpCnt = Random.Range(1, max_jump); }
        else
        { iJumpCnt = _cnt; }

        List<EJumpType> temp = new List<EJumpType>();
        for (int i = 0; i < iJumpCnt; ++i)
        {
            EJumpType type = GetJump(Random.Range(0, (int)EJumpType.NUM));
            temp.Add(type);
        }

        return temp;
    }

    /// <summary>
    /// 사용안함
    /// </summary>
    /// <returns></returns>
    EState CheckDirection()
    {
        int preJumpCnt = 0;
        EState dir = Random.Range(0, 99) % 2 == 0 ? EState.LEFT : EState.RIGHT;

        if (dir == preDir)
            dir = (preDir != EState.LEFT) ? EState.RIGHT : EState.LEFT;

        switch (dir)
        {
            case EState.LEFT:
                preJumpCnt = GetNumBit(bitLeft);
                break;
            case EState.RIGHT:
                preJumpCnt = GetNumBit(bitRight);
                break;
        }

        if (max_jump - preJumpCnt == 0)
        {
            dir = (dir == EState.LEFT) ? EState.RIGHT : EState.LEFT;
            iJumpCnt = Random.Range(1, max_jump + 1);
        }
        else
        {   iJumpCnt = max_jump - preJumpCnt;  }

        if (mJumptype == EJumpType.HOVER) iJumpCnt = 1;
        return dir;
    }

    /// <summary>
    /// 피격시
    /// </summary>
    void HitEffect()
    {
        float hpRatio = (float)iLife / fMaxHp;
        hpBar.UpdateDisplay(hpRatio);
        hpBar.Hit();

        // 피격 시 몸체 색상 변경
        if (hpRatio < 0.5f)
        { render.material.SetFloat("_Risks", 1 - hpRatio); }

        ani.SetTrigger("Hit");
        ParticleEmitt(EParticleType.HIT, hitSnd, cachedTransform.position);
    }

    /// <summary>
    /// 파티클 생성
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="_snd"></param>
    /// <param name="_pos"></param>
    void ParticleEmitt(EParticleType _type, AudioClip _snd, Vector3 _pos)
    {
        _pos = controller.WorldToBarrelScreen(_pos);
        _pos.y += 0.2f;
        particle.ParticleEmitt(_type, _pos);
        ML.PlaywallKids.Common.AudioPlay2D.PlayClip(_snd);
        //AudioSource.PlayClipAtPoint(_snd, _pos);
    }

    int GetNumBit(int bit)
    {
        int cnt = 0;
        for (int i = 0; i < 32; ++i)
        {
            if ((bit & 0x01 << i) != 0)
            { cnt++; }
            else
            { return cnt; }

        }
        return 0;
    }

    void CheckBit(EState _dir)
    {
        switch (_dir)
        {
            case EState.LEFT:
                if (bitRight != 0)
                { ReleaseBit(ref bitRight); }
                else
                { SetBit(ref bitLeft); }
                break;
            case EState.RIGHT:
                if (bitLeft != 0)
                { ReleaseBit(ref bitLeft); }
                else
                { SetBit(ref bitRight); }
                break;
        }
    }

    void SetBit(ref int bit)
    {
        for (int i = 0; ; ++i)
        {
            if ((bit & 0x01 << i) != 0)
            { continue; }
            else
            {
                bit |= 0x01 << i;
                return;
            }
        }
    }

    void ReleaseBit(ref int dir)
    {
        for (int i = 0; ; ++i)
        {
            if ((dir & 0x01 << i) != 0)
            { continue; }
            else
            {
                if (i - 1 >= 0)
                { dir &= ~(0x01 << (i - 1)); }
                return;
            }
        }
    }
    #endregion

    #region Coroutine Func
    IEnumerator DeadProcess()
    {
        if (hpBar != null)
        { hpBar.gameObject.SetActive(false); }
        ParticleEmitt(EParticleType.KO, deadSnd, cachedTransform.position);
        bCollider = false;
        ani.SetTrigger("Die");
        ani.SetBool("Idle", false);
        obj.SetActive(false);
        yield return new WaitForSeconds(1f);
        //   this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    IEnumerator BezierJumpProcess(Vector3 target, int _jumpCnt)
    {
        bCollider = false;
        fActiveTime = 0;
        int currentJump = 0;
        float width = jumpWidth * (currentState == EState.LEFT ? -1 : 1);

        int n = (mJumptype == EJumpType.ZIGZAG) ? -1 : 1;

        Bezier myBezier = new Bezier(cachedTransform.localPosition, bezierHandle, n * bezierHandle, target);
        do
        {
            cachedTransform.localPosition = myBezier.GetPointAtTime(fActiveTime);
            fActiveTime += Time.deltaTime;

            if (fActiveTime > 1f) // arrived to target
            {
                fActiveTime = 0f;
                cachedTransform.localPosition = target;
                target.x = width + cachedTransform.localPosition.x;
                myBezier = null;
                myBezier = new Bezier(cachedTransform.localPosition, bezierHandle, n * bezierHandle, target);
                CheckBit(currentState);
                currentJump++;
            }

            yield return new WaitForEndOfFrame();
        } while (_jumpCnt > currentJump);
        myBezier = null;
        preDir = currentState;
        currentState = EState.IDLE;
    }

    /// <summary>
    /// 현재 사용안함
    /// </summary>
    /// <returns></returns>
    IEnumerator CloneProcess()
    {
        for (int i = 0; i < 3; ++i)
        {
            ParticleEmitt(EParticleType.ARRIVE, arriveSnd, cachedTransform.position);
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < 2; ++i)
        {
            GameObject go = Instantiate(this.gameObject) as GameObject;
            TouchMotionBossObject clone = go.GetComponent<TouchMotionBossObject>();

            clone.cachedTransform.localPosition = cachedTransform.localPosition;
            clone.currentState = EState.JUMP;
            clone.bCollider = false;
            clone.cachedTransform.parent = cachedTransform.parent;
        }
        yield return new WaitForEndOfFrame();
        bCollider = true;
    }

    IEnumerator HitProcess()
    {
        HitEffect();
        yield return new WaitForSeconds(0.03f);

        ani.SetBool("Hit", false);
        currentState = EState.IDLE;
    }

    /// <summary>
    /// 보스 슬라임 연속 점프 
    /// </summary>
    /// <param name="_jumpCnt"></param>
    /// <returns></returns>
    IEnumerator JumpProcess(int _jumpCnt = 0)
    {
        if (jumpList != null)
        {
            jumpList.Clear();
            jumpList = null;
        }

        jumpList = SetJumpList(_jumpCnt);
        bCollider = true;
        fActiveTime = 0;
        int currentJump = 0;
        float size = cachedTransform.localScale.x;

        // 목적 좌표
        Vector3 target = new Vector3(Random.Range(minX + size / 2, maxX - size / 2),
                                     Random.Range(minY, maxY - jumpHeight),
                                     cachedTransform.localPosition.z);

        mJumptype = jumpList[currentJump];
        int n = (mJumptype == EJumpType.ZIGZAG ? -1 : 1);

        Bezier myBezier = new Bezier(cachedTransform.localPosition, bezierHandle, n * bezierHandle, target);
        do
        {
            //베지어 계산 된 좌표
            cachedTransform.localPosition = myBezier.GetPointAtTime(fActiveTime);
            fActiveTime += Time.deltaTime * jumpSpeed;
            // 점프 중 피격시
            if (currentState == EState.JUMPING_HIT)
            {
                HitEffect();
                currentState = EState.JUMP;
            }

            // 점프할 장소로 도착함
            if (fActiveTime > 1f) // arrived to target
            {
                fActiveTime = 0f;

                Vector3 pos = cachedTransform.position;
                pos.y -= 1.5f;
                // 도착 파티클 생성
                ParticleEmitt(EParticleType.ARRIVE, arriveSnd, pos);

                cachedTransform.localPosition = target;
                target = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), cachedTransform.localPosition.z);
                ++currentJump;
                // 다음 점프
                if (currentJump < jumpList.Count)
                {
                    mJumptype = jumpList[currentJump];

                    if (currentJump == jumpList.Count - 1)//마지막 점프 카운트일 경우 화면 아래위치로 강제 설정
                    { target.y = minY; }

                    n = (mJumptype == EJumpType.ZIGZAG ? -1 : 1);
                    myBezier = null;
                    myBezier = new Bezier(cachedTransform.localPosition, bezierHandle, n * bezierHandle, target);
                }
            }
            yield return new WaitForEndOfFrame();
        } while (iJumpCnt > currentJump);

        currentState = EState.IDLE;
    }

    #endregion


}


/* IEnumerator JumpProcess(int _jumpCnt = 0)
    {
        if (jumpList != null)
        {
            jumpList.Clear();
            jumpList = null;
        }

        jumpList        = SetJumpList(_jumpCnt);
        bCollider       = false;
        fActiveTime     = 0;
        int currentJump = 0;
        float size = cachedTransform.localScale.x;

        Vector3 target = new Vector3(Random.Range(minX + size/2, maxX - size/2),
                                     Random.Range(minY , maxY - jumpHeight),
                                     cachedTransform.localPosition.z);

        mJumptype = jumpList[currentJump];
       int n = (mJumptype == EJumpType.ZIGZAG ? -1 : 1);

        Bezier myBezier = new Bezier(cachedTransform.localPosition, bezierHandle, n * bezierHandle, target);
        do
        {
            cachedTransform.localPosition = myBezier.GetPointAtTime(fActiveTime);
            fActiveTime += Time.deltaTime * jumpSpeed;
            
            if (fActiveTime > 1f) // arrived to target
            {
                fActiveTime = 0f;
                AudioSource.PlayClipAtPoint(arriveSnd, Vector3.zero,1f);

                GameObject particle = Instantiate(arriveParticle) as GameObject; //create particle
                particle.transform.parent = cam.transform;
                Vector3 pos = cachedTransform.forward - cachedTransform.localPosition ;
                pos.y -= cachedTransform.localScale.y * 0.1f;
                pos.z += particle.transform.parent.transform.localPosition.z * 1.5f;
                particle.transform.localPosition = pos;
                particle.transform.localScale = Vector3.one;
                particle.transform.localRotation = Quaternion.Euler(Vector3.zero);
                cachedTransform.localPosition = target;
                target = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), cachedTransform.localPosition.z);
                currentJump++;
                if (currentJump < jumpList.Count)
                { 
                    mJumptype = jumpList[currentJump];
                    if (currentJump == jumpList.Count - 1)
                    { target.y = minY; }
                    n = (mJumptype == EJumpType.ZIGZAG ? -1 : 1);
                    myBezier = null;
                    myBezier = new Bezier(cachedTransform.localPosition, bezierHandle, n * bezierHandle, target);
                }
            }
            yield return new WaitForEndOfFrame();
        } while (iJumpCnt > currentJump);

        currentState = EState.IDLE;
    }
*/