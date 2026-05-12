using UnityEngine;

namespace ML.PlaywallKids.MotionJump
{
    /// <summary>
    /// 플레이어 아바타
    /// </summary>
    public class JumpPlayerController : MonoBehaviour
    {
        //public KeyCode key;
        /// <summary>
        /// 점프 힘
        /// </summary>
        [Range(1f, 1000f)]
        public float jumpStrength;
        /// <summary>
        /// 측면 이동 속도 
        /// </summary>
        [Range(0.1f, 10f)]
        public float moveHorizontalSpeed;
        //public bool initializeVelocity;
        public enum Estate
        { NONE, READY, JUMPING, FALLING, BE_REACHED }
        /// <summary>
        /// BurstParticle GameOjbect
        /// </summary>
        public GameObject boostEffect;
        /// <summary>
        /// sndJump1
        /// </summary>
        public AudioClip sndJump;
        //public AudioClip sndSonicBoom;
        #region Property
        private Estate mState;
        public Estate currentState
        {
            protected set
            {
                if (value != mState)
                {
                    switch (value)
                    {
                        case Estate.NONE:

                            break;

                        case Estate.JUMPING:
                            Animator.enabled = false;
                            break;
                        case Estate.FALLING:
                            break;
                        case Estate.READY:
                            SetAnimation("Dance", false);
                            SetAnimation("Lose", false);

                            CachedTransform.localPosition = Vector3.zero;
                            if (!Rigid.useGravity) Rigid.useGravity = true;
                            arriveParticle.gameObject.SetActive(false);
                            boostEffect.gameObject.SetActive(false);
                            break;
                        case Estate.BE_REACHED:

                            SetAnimation("Dance", true);
                            Rigid.velocity = Vector3.zero;
                            Rigid.useGravity = false;
                            break;
                    }
                    mState = value;
                }
            }

            get
            { return mState; }
        }

        private Rigidbody mRigid;
        public Rigidbody Rigid
        {
            get
            {
                if (mRigid == null)
                {
                    mRigid = this.GetComponent<Rigidbody>();
                    if (mRigid == null)
                    {
                        this.gameObject.AddComponent<Rigidbody>();
                        mRigid = this.GetComponent<Rigidbody>();
                    }
                }
                return mRigid;
            }
        }

        private JumpCameraController mCamController;
        /// <summary>
        /// 현재 사용 안함
        /// </summary>
        public JumpCameraController CamController
        {
            get
            {
                if (mCamController == null)
                { mCamController = CachedTransform.parent.GetComponentInChildren<JumpCameraController>(); }
                return mCamController;
            }
        }

        /// <summary>
        /// 현재 아바타 높이
        /// </summary>
        public float Height
        {
            get
            { return CachedTransform.localPosition.y; }
        }

        public Transform CachedTransform
        {
            get
            {
                if (mTransform == null)
                { mTransform = this.transform; }
                return mTransform;
            }
        }

        /// <summary>
        /// 획득한 코인 개수
        /// </summary>
        public int CntCoin
        {
            get;
            private set;
        }

        /// <summary>
        /// 바닥에 닿으면 true : GameOver
        /// </summary>
        public bool OnTheFloor
        { get; private set; }

        private bool bInput
        {
            get { return (currentState != Estate.BE_REACHED && usingItem == EItemType.NONE); }
        }

        private Animator mAnimator;
        public Animator Animator
        {
            get
            {
                if (mAnimator == null)
                { mAnimator = this.GetComponentInChildren<Animator>(); }
                return mAnimator;
            }
        }


        private AvatarSkeleton mSkeleton;
        /// <summary>
        /// Kinect 사용자 모션 정보 -> 아바타와 연동
        /// </summary>
        public AvatarSkeleton skeleton
        {
            get
            {
                if (mSkeleton == null)
                { mSkeleton = this.GetComponentInChildren<AvatarSkeleton>(); }
                return mSkeleton;
            }
        }

        /// <summary>
        /// CFX_Sole_Smoke
        /// </summary>
        public ParticleSystem arriveParticle;

        #endregion

        #region Private variable
        private Transform mTransform;
        private float fActiveItemTime;
        private EItemType usingItem;
        #endregion

        #region Unity Func
        /// <summary>
        /// 아이템 충돌 이벤트
        /// </summary>
        /// <param name="_other"></param>
        void OnTriggerEnter(Collider _other)
        {
            Item item = _other.GetComponent<Collider>().gameObject.GetComponent<Item>();
            if (item != null)
            {

                switch (item.type)
                {
                    case EItemType.SPEED:
                        boostEffect.SetActive(true);
                        //AudioSource.PlayClipAtPoint(sndSonicBoom, Vector3.zero);
                        Jump(item.strength);
                        usingItem = item.type;
                        Rigid.velocity = Vector3.zero;
                        break;

                    case EItemType.COIN:
                        if (usingItem != EItemType.SPEED)
                        {
                            Rigid.velocity = Vector3.zero;
                            Jump(item.strength);
                            usingItem = item.type;
                        }
                        CntCoin += 1;
                        break;
                }

                fActiveItemTime = 0;
                item.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 바닥 충돌 체크
        /// </summary>
        /// <param name="_other"></param>
        void OnCollisionEnter(Collision _other)
        {
            bool condition = (currentState == Estate.JUMPING || currentState == Estate.FALLING);
            bool falling = Rigid.velocity.y <= 0f;
            if (condition && falling)
            {
                OnTheFloor = true;
                SetAnimation("Lose", true);
            }
        }

        void FixedUpdate()
        {
            CheckItemState();
        }
        #endregion

        #region Private Func
        void SetAnimation(string _name, bool _value)
        {
            if (!Animator.enabled)
                Animator.enabled = true;
            Animator.SetBool(_name, _value);
        }

        /// <summary>
        /// 제자리에 가만히 있기
        /// </summary>
        /// <param name="_strength"></param>
        void Freeze(float _strength)
        {
            fActiveItemTime = _strength;
        }

        void Jump(float _strength)
        {
            //속도 초기화
            if (Rigid.velocity.y > 0) Rigid.velocity = Vector3.zero;

            Rigid.AddForce(Vector3.up * _strength, ForceMode.Force);
            currentState = Estate.JUMPING;
            Common.AudioPlay2D.PlayClip(sndJump);

            //점프 파티클 동적 생성
            GameObject particle = Instantiate(arriveParticle.gameObject) as GameObject;
            Vector3 pos = CachedTransform.position;
            pos.y += 0.6f;
            // 파티클 높이 적용
            particle.transform.position = pos;

            particle.gameObject.SetActive(true);
            Destroy(particle, 3.0f);
        }

        void CheckItemState()
        {
            switch (usingItem)
            {
                case EItemType.FREEZE:

                    break;

                case EItemType.SPEED:

                    if (fActiveItemTime != 0 && Rigid.velocity.y <= 0) //속도가 저하
                    {
                        boostEffect.SetActive(false);

                        usingItem = EItemType.NONE;
                    }
                    fActiveItemTime += Time.fixedDeltaTime;
                    break;
                default: usingItem = EItemType.NONE; return;
            }
        }
        #endregion

        #region Public Func
        public void Init()
        {
            OnTheFloor = false;
            usingItem = EItemType.NONE;
            currentState = Estate.READY;
            CntCoin = 0;
            //        animator.Play("aniCelebrations", -1, 0f);
            //        AnimationInfo[] aniInfos = animator.GetCurrentAnimationClipState(0);
        }

        public void BeArrived()
        {
            currentState = Estate.BE_REACHED;
        }

        public void Jump()
        {
            if (bInput)
            { Jump(jumpStrength); }
        }

        public void MoveHorizontal(bool _bRight)
        {
            Vector3 pos = CachedTransform.localPosition;
            pos.x += Time.deltaTime * (_bRight ? 1 : -1) * moveHorizontalSpeed;
            CachedTransform.localPosition = pos;
        }
        #endregion
    }
}