using GuessNumber;
using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class CarController : MonoBehaviour
    {
        #region Enumerations
        internal enum DriveType
        {
            FrontWheelDrive,
            RearWheelDrive,
            AllWheelDrive
        }
        [SerializeField] DriveType driveType;

        internal enum GearBox
        {
            automatic,
            manual
        }
        [SerializeField] GearBox gearbox;
        #endregion

        #region Fields and Properties

        [Header("������ �ӵ� �� ���� Ư��")]
        private float kmH;
        public float KmH
        {
            get { return kmH; }
            set
            {
                float clampedValue = Mathf.Clamp(value, 0, 1000); // �ӵ� ����
                kmH = clampedValue;

                CheckZeroSpeedAndReset();
            }
        }

        private float brakePower;
        [HideInInspector] public float totalPower;
        private float wheelsRPM;
        private float radius = 0f;

        [Header("���� ����")]
        private float smoothTime = 0.09f;
        [SerializeField] private AnimationCurve enginePower;
        [HideInInspector] public float engineRPM;

        [Header("���ڽ� ����")]
        [HideInInspector] public bool reverse;
        [SerializeField] private float[] gears;
        [SerializeField] private float[] gearChangeSpeed;
        [HideInInspector] public float maxRPM = 8000f, minRPM = 5000f;
        [HideInInspector] public int gearNumber = 1;

        [Header("�帮��Ʈ ����")]
        private WheelFrictionCurve forwardFriction, sidewaysFriction;
        private float driftFactor;
        private float handBrakeFrictionMultiplier = 2f;
        [HideInInspector] public bool isDrifting = false;
        private float driftStartTime = 0f;
        private float driftDurationThreshold = 0.3f;

        [Header("����� ����")]
        private float[] slips = new float[4];

        [Header("Unity ������Ʈ �� ����")]
        private InputManager inputManager;
        private CarEffect carEffect;
        [HideInInspector] public Rigidbody rb;
        private ObjectFade fade;

        [Header("���� ����")]
        private GameObject wheelsParent, wheelMeshesParent;
        private GameObject centerOfMass;
        private WheelCollider[] wheels = new WheelCollider[4];
        private GameObject[] wheelMeshes = new GameObject[4];

        [Header("���� ���� �Ӽ�")]
        private float wheelBase;
        private float trackWidth;
        public float downForceValue = 1000f;

        [Header("�ν��� ����")]
        [HideInInspector] public float boostValue;
        [HideInInspector] public bool boostFlag = false;
        private Coroutine boostCoroutine;
        [HideInInspector] public float remainingBoostTime = 0f;

        [Header("��Ÿ ���� ����")]
        private bool flag = false;
        [HideInInspector] public bool test;
        private float lastValue;
        public float radiusScale = 10f;
        [HideInInspector] public float vertical = 1f;
        [HideInInspector] public float horizontal;

        [Header("Reset ����")]
        [HideInInspector] public Transform resetPoint;
        private float resetDuration = 3f;
        private float rayDistance = 100f;
        private bool isOutOfTrack = false;
        private float outOfTrackDuration = 0f; // Ʈ�� ��� ���� �ð� ����
        private string trackTag = "Track";
        private string guardRailTag = "GuardRail";
        private bool isZeroSpeed = false;
        private float zeroSpeedDuration = 0f; // ���� �ӵ��� 0�� ���°� �ð� ����
        [HideInInspector] public Vector3 currentTrackDirection;
        private float reverseDirectionDuration = 0f; // �ݴ� ���� ���� �ð� ����
        private bool isResetting = false;

        #endregion


        #region Unity Callbacks
        private void Awake()
        {
            GetObjects();
            CalculateWheelBaseAndTrackWidth();
            StartCoroutine(TimedLoopRoutine());
        }

        private void Update()
        {
            ClaculateEnginePower();
            CheckOnTrackAndReset();
            CheckDirection();

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetToLastCheckpoint();
            }
        }

        private void FixedUpdate()
        {
            lastValue = engineRPM;

            AddDownForce();
            AnimateWheels(); // �� �ִϸ��̼�
            SteeringWheel();
            GetFriction();
            AdjustTraction();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("GuardRail"))
            {
                SoundMGR.Instance.SoundPlay("Collision");
            }
        }

        #endregion
        public void StartEngineSound()
        {
            StartCoroutine(StartSoundRoutine());
        }

        IEnumerator StartSoundRoutine()
        {
            SoundMGR.Instance.SoundPlay("EngineStart");

            yield return new WaitForSeconds(2f);

            SoundMGR.Instance.SoundPlay("Idle");
            SoundMGR.Instance.SoundPlay("Running");
        }

        #region Movement and Controls
        private void MoveVehicle()
        {
            //BrakeVehicle();

            switch (driveType)
            {
                case DriveType.AllWheelDrive:
                    for (int i = 0; i < wheels.Length; i++)
                    {
                        wheels[i].motorTorque = totalPower / 4;
                    }
                    break;

                case DriveType.RearWheelDrive:
                    for (int i = 2; i < wheels.Length; i++)
                    {
                        wheels[i].motorTorque = totalPower / 2;
                    }
                    break;

                case DriveType.FrontWheelDrive:
                    for (int i = 0; i < wheels.Length - 2; i++)
                    {
                        wheels[i].motorTorque = totalPower / 2;
                    }
                    break;

                default:
                    break;
            }

            KmH = rb.velocity.magnitude * 3.6f;
        }

        private void BrakeVehicle()
        {
            if (vertical < 0)
            {
                brakePower = (KmH >= 10) ? 500 : 0;
            }
            else if (vertical == 0 && (KmH <= 10 || KmH >= -10))
            {
                brakePower = 10;
            }
            else
            {
                brakePower = 0;
            }
        }

        private void SteeringWheel()
        {
            if (horizontal > 0)
            {
                wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (trackWidth / 2))) * horizontal;
                wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (trackWidth / 2))) * horizontal;
            }
            else if (horizontal < 0)
            {
                wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius - (trackWidth / 2))) * horizontal;
                wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (radius + (trackWidth / 2))) * horizontal;
            }
            else
            {
                wheels[0].steerAngle = 0;
                wheels[1].steerAngle = 0;
            }
        }
        #endregion

        #region Wheels
        private void AnimateWheels()
        {
            Vector3 wheelPosition = Vector3.zero;
            Quaternion wheelRotation = Quaternion.identity;

            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
                wheelMeshes[i].transform.position = wheelPosition;
                wheelMeshes[i].transform.rotation = wheelRotation;
            }
        }

        private void GetFriction()
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                WheelHit wheelHit;
                wheels[i].GetGroundHit(out wheelHit);

                slips[i] = wheelHit.forwardSlip;
            }
        }

        private void ProvideFrontWheelGrip()
        {
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
            forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;

            for (int i = 0; i < 2; i++) // �չ����� ����
            {
                wheels[i].forwardFriction = forwardFriction;
                wheels[i].sidewaysFriction = sidewaysFriction;
            }
        }

        private void ApplyFrictionToAllWheels(WheelFrictionCurve forward, WheelFrictionCurve sideways)
        {
            foreach (var wheel in wheels)
            {
                wheel.forwardFriction = forward;
                wheel.sidewaysFriction = sideways;
            }
        }
        #endregion

        #region Engine and Transmission
        private void ClaculateEnginePower()
        {
            WheelRPM();

            if (vertical != 0)
            {
                rb.drag = 0.005f;
            }
            if (vertical == 0)
            {
                rb.drag = 0.1f;
            }
            totalPower = 3.6f * enginePower.Evaluate(engineRPM) * vertical;

            float velocity = 0.0f;
            if (engineRPM >= maxRPM || flag)
            {
                engineRPM = Mathf.SmoothDamp(engineRPM, maxRPM - 500, ref velocity, 0.05f);

                flag = (engineRPM >= maxRPM - 450) ? true : false;
                test = (lastValue > engineRPM) ? true : false;
            }
            else
            {
                engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) * 3.6f * (gears[gearNumber])), ref velocity, smoothTime);
                test = false;
            }

            if (engineRPM >= maxRPM + 1000) engineRPM = maxRPM + 1000; // clamp at max

            MoveVehicle();
            Shifter();
        }

        private void WheelRPM()
        {
            float sum = 0;
            int R = 0;

            for (int i = 0; i < wheels.Length; i++)
            {
                sum += wheels[i].rpm;
                R++;
            }

            wheelsRPM = (R != 0) ? sum / R : 0;

            if (wheelsRPM < 0 && !reverse)
            {
                reverse = true;
                GameManager.Instance.ChangeGear();
            }
            else if (wheelsRPM > 0 && reverse)
            {
                reverse = false;
                GameManager.Instance.ChangeGear();
            }
        }

        private void Shifter()
        {
            if (!IsGrounded()) return;

            switch (gearbox)
            {
                case GearBox.automatic:
                    if (engineRPM > maxRPM && gearNumber < gears.Length - 1 && !reverse && CheckGears())
                    {
                        gearNumber++;
                        GameManager.Instance.ChangeGear();
                        return; // ��� ���� �� ���� ����
                    }
                    else if (engineRPM < minRPM && gearNumber > 0)
                    {
                        gearNumber--;
                        GameManager.Instance.ChangeGear();
                    }
                    break;

                case GearBox.manual:
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        gearNumber++;
                        GameManager.Instance.ChangeGear();
                    }
                    else if (engineRPM < minRPM && gearNumber > 0)
                    {
                        gearNumber--;
                        GameManager.Instance.ChangeGear();
                    }
                    break;
            }
        }
        #endregion

        #region Drift and Traction
        private void AdjustTraction()
        {
            float driftSmoothFactor = 0.7f * Time.deltaTime;
            float velocity = 0;

            sidewaysFriction = wheels[0].sidewaysFriction;
            forwardFriction = wheels[0].forwardFriction;

            if (inputManager.HandBrake)
            {
                float targetFriction = driftFactor * handBrakeFrictionMultiplier;
                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                    Mathf.SmoothDamp(forwardFriction.asymptoteValue, targetFriction, ref velocity, driftSmoothFactor);

                ApplyFrictionToAllWheels(forwardFriction, sidewaysFriction);
                ProvideFrontWheelGrip();
                ApplyForwardForce();
            }
            else
            {
                float restoredFriction = ((KmH * handBrakeFrictionMultiplier) / 300) + 1f;
                forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = restoredFriction;

                ApplyFrictionToAllWheels(forwardFriction, sidewaysFriction);
            }

            UpdateDriftStateAndSmoke();
        }

        private void UpdateDriftStateAndSmoke()
        {
            bool wasDrifting = isDrifting;
            isDrifting = false;

            for (int i = 2; i < 4; i++)
            {
                WheelHit wheelHit;
                wheels[i].GetGroundHit(out wheelHit);

                if (kmH >= 80f && (Mathf.Abs(wheelHit.sidewaysSlip) >= 0.5f || Mathf.Abs(wheelHit.forwardSlip) >= 0.5f))
                {
                    isDrifting = true;

                    if (!wasDrifting)
                    {
                        driftStartTime = Time.time;
                    }
                }

                if (wheelHit.sidewaysSlip != 0)
                {
                    driftFactor = (1 + Mathf.Sign(wheelHit.sidewaysSlip) * horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);
                }
            }

            if (inputManager.HandBrake == false && wasDrifting && !isDrifting)
            {
                float driftDuration = Time.time - driftStartTime;

                if (driftDuration >= driftDurationThreshold)
                {
                    ActivateBoost(2000f, 0.5f);
                }
            }
        }

        private void ApplyForwardForce()
        {
            rb.AddForce(transform.forward * (KmH / 400) * 10000);
        }
        #endregion

        #region Boost
        public void ActivateBoost(float force, float duration)
        {
            remainingBoostTime += duration;

            if (boostCoroutine != null)
            {
                boostFlag = false;
                StopCoroutine(boostCoroutine);
            }
            boostCoroutine = StartCoroutine(BoostRoutine(force));
        }

        private IEnumerator BoostRoutine(float force)
        {
            boostFlag = true;

            while (remainingBoostTime > 0f)
            {
                rb.AddForce(transform.forward * force * Time.deltaTime, ForceMode.Acceleration);
                remainingBoostTime -= Time.deltaTime;
                yield return null;
            }

            boostFlag = false;
            boostCoroutine = null;
        }
        #endregion

        #region Utility
        private bool CheckGears()
        {
            return KmH >= gearChangeSpeed[gearNumber];
        }

        private void GetObjects()
        {
            inputManager = GetComponent<InputManager>();
            carEffect = GetComponent<CarEffect>();
            rb = GetComponent<Rigidbody>();
            fade = GetComponentInChildren<ObjectFade>();

            wheelsParent = GameObject.Find("WheelColliders");
            wheels[0] = wheelsParent.transform.Find("FrontLeft").gameObject.GetComponent<WheelCollider>();
            wheels[1] = wheelsParent.transform.Find("FrontRight").gameObject.GetComponent<WheelCollider>();
            wheels[2] = wheelsParent.transform.Find("RearLeft").gameObject.GetComponent<WheelCollider>();
            wheels[3] = wheelsParent.transform.Find("RearRight").gameObject.GetComponent<WheelCollider>();

            wheelMeshesParent = GameObject.Find("WheelMeshes");
            wheelMeshes[0] = wheelMeshesParent.transform.Find("FrontLeft").gameObject;
            wheelMeshes[1] = wheelMeshesParent.transform.Find("FrontRight").gameObject;
            wheelMeshes[2] = wheelMeshesParent.transform.Find("RearLeft").gameObject;
            wheelMeshes[3] = wheelMeshesParent.transform.Find("RearRight").gameObject;

            centerOfMass = GameObject.Find("CenterOfMass");
            rb.centerOfMass = centerOfMass.transform.localPosition;
        }

        private void CalculateWheelBaseAndTrackWidth()
        {
            trackWidth = Vector3.Distance(wheels[2].transform.position, wheels[3].transform.position);
            wheelBase = Vector3.Distance(wheels[0].transform.position, wheels[2].transform.position);
        }

        private IEnumerator TimedLoopRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(.7f);
                radius = 6f + (KmH / radiusScale);
            }
        }

        private void AddDownForce()
        {
            if (IsGrounded())
            {
                rb.AddForce(-transform.up * downForceValue * rb.velocity.magnitude);
            }
            else
            {
                rb.AddForce(Vector3.down * downForceValue * rb.velocity.magnitude);
            }

        }
        #endregion

        #region Ground Check & Reset

        public bool IsGrounded()
        {
            // ��� ������ ���� ������ Ȯ��
            foreach (var wheel in wheels)
            {
                WheelHit hit;
                if (wheel.GetGroundHit(out hit))
                {
                    return true; // �ϳ��� �����Ǹ� Grounded ����
                }
            }
            return false; // ��� ������ ���߿� �� ����
        }

        private void CheckOnTrackAndReset()
        {
            //if (IsGrounded()) return;

            Ray ray = new Ray(transform.position, -transform.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.collider.CompareTag(trackTag) || hit.collider.CompareTag(guardRailTag))
                {
                    //Debug.DrawRay(transform.position, -transform.up * rayDistance, Color.green);

                    // Ʈ�� ���� ���� ��� ���� �ʱ�ȭ
                    if (isOutOfTrack)
                    {
                        isOutOfTrack = false;
                        outOfTrackDuration = 0f;
                    }
                    return;
                }
                else
                {
                    //Debug.DrawRay(transform.position, -transform.up * rayDistance, Color.red);

                    // Ʈ�� �ܺ� ����
                    if (!isOutOfTrack)
                    {
                        isOutOfTrack = true;
                    }

                    outOfTrackDuration += Time.deltaTime;

                    if (outOfTrackDuration >= resetDuration + 1)
                    {
                        //Debug.Log("Ʈ�� �ܺ� ���� �������� ���� ����!");
                        ResetToLastCheckpoint();
                    }
                }
            }
            else
            {
                Debug.DrawRay(transform.position, Vector3.down * rayDistance, Color.yellow);

                // ���߿� �� �ִ� ����
                if (!isOutOfTrack)
                {
                    isOutOfTrack = true;
                }

                outOfTrackDuration += Time.deltaTime;

                if (outOfTrackDuration >= resetDuration + 1)
                {
                    //Debug.Log("���� ���� �������� ���� ����!");
                    ResetToLastCheckpoint();
                    outOfTrackDuration = 0f; // ���� �� �ð� �ʱ�ȭ
                }
            }
        }

        private void CheckZeroSpeedAndReset()
        {
            if (kmH <= 1f)
            {
                if (!isZeroSpeed)
                {
                    isZeroSpeed = true;
                }

                zeroSpeedDuration += Time.deltaTime;

                if (zeroSpeedDuration >= resetDuration)
                {
                    //Debug.Log("���� �ӵ� 0 ������ ���� ����!");
                    ResetToLastCheckpoint();
                }
            }
            else
            {
                if (isZeroSpeed)
                {
                    isZeroSpeed = false;
                    zeroSpeedDuration = 0f;
                }
            }
        }

        public void ResetToLastCheckpoint()
        {
            if (isResetting) return;
            isResetting = true;

            if (resetPoint == null)
            {
                Debug.LogWarning("üũ����Ʈ�� �������� �ʾҽ��ϴ�!");
                isResetting = false;
                return;
            }

            // ��ġ �ʱ�ȭ
            boostFlag = false;
            remainingBoostTime = 0f;
            reverseDirectionDuration = 0f;
            zeroSpeedDuration = 0f;
            outOfTrackDuration = 0f; // ���� �� �ð� �ʱ�ȭ
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            downForceValue = 1000f;
            radiusScale = 10f;

            // ������ ������ üũ����Ʈ�� �̵�
            transform.position = resetPoint.position;
            transform.rotation = resetPoint.rotation;

            // ���� �ʱ�ȭ
            currentTrackDirection = transform.forward;

            // �����̱� ȿ�� ����
            StartCoroutine(BlinkEffect(3f, 0.5f)); // 3�� ���� ������, �� �������� 0.5��
            SoundMGR.Instance.SoundPlay("Reset");

            isResetting = false;
        }

        private IEnumerator BlinkEffect(float duration, float blinkInterval)
        {
            float elapsedTime = 0f;

            carEffect.CarResetStart(duration);

            while (elapsedTime < duration)
            {
                // ���̵� �ƿ�
                fade.FadeOut(blinkInterval / 2);
                yield return new WaitForSeconds(blinkInterval / 2);

                // ���̵� ��
                fade.FadeIn(blinkInterval / 2);
                yield return new WaitForSeconds(blinkInterval / 2);

                elapsedTime += blinkInterval;
            }

            // ������ ���� �� ������ ǥ��
            fade.FadeIn(0.1f);
        }

        private void CheckDirection()
        {
            Vector3 vehicleDirection = transform.forward.normalized;
            float dot = Vector3.Dot(vehicleDirection, currentTrackDirection);

            if (dot < 0) // �ݴ� ����
            {
                reverseDirectionDuration += Time.deltaTime;

                if (reverseDirectionDuration >= resetDuration + 2)
                {
                    //Debug.Log("�ݴ� �������� ���� �ð��� �ʰ��Ǿ� ���µ˴ϴ�.");
                    ResetToLastCheckpoint();
                }
            }
            else // �ùٸ� ���� �Ǵ� ���� ����
            {
                reverseDirectionDuration = 0f; // �ݴ� ���� ���� �ð� �ʱ�ȭ
            }
        }

        #endregion
    }
}
