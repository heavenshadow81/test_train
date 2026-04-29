using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class CarEffect : MonoBehaviour
    {
        //[SerializeField] ParticleSystem[] smokes = new ParticleSystem[2];
        [SerializeField] private TrailRenderer[] skidMarks = new TrailRenderer[2];
        [SerializeField] private GameObject[] newSkidMarks = new GameObject[4];
        [SerializeField] GameObject[] boosterEffects = null;
        [SerializeField] GameObject aeroEffect = null;
        [SerializeField] CameraController cameraController = null;
        private CarController carController;
        //private bool smokeFlag = false;

        private Dictionary<Transform, Vector3> initialScales = new Dictionary<Transform, Vector3>(); // 초기 스케일 저장소

        private void Start()
        {
            carController = GetComponent<CarController>();

            // 부스터의 초기 스케일 저장
            foreach (GameObject boosterEffect in boosterEffects)
            {
                Transform boosterTransform = boosterEffect.transform;
                if (!initialScales.ContainsKey(boosterTransform))
                {
                    initialScales.Add(boosterTransform, boosterTransform.localScale);
                }
            }
        }

        private void FixedUpdate()
        {
            //ActivateSmoke();
            ActivateSkid();
            ActivateBoostEffect(carController.boostFlag);
        }

        #region "스모크"
        //private void ActivateSmoke()
        //{
        //    if (carController.isDrifting) StartSmoke();
        //    else StopSmoke();

        //    if (smokeFlag)
        //    {
        //        for (int i = 0; i < smokes.Length; i++)
        //        {
        //            var emission = smokes[i].emission;
        //            emission.rateOverTime = ((int)carController.KmH * 10 <= 2000) ? (int)carController.KmH * 10 : 2000;
        //        }
        //    }
        //}

        //public void StartSmoke()
        //{
        //    if (smokeFlag) return;
        //    for (int i = 0; i < smokes.Length; i++)
        //    {
        //        var emission = smokes[i].emission;
        //        emission.rateOverTime = ((int)carController.KmH * 2 >= 2000) ? (int)carController.KmH * 2 : 2000;
        //        smokes[i].Play();
        //    }
        //    smokeFlag = true;
        //    StartSkid();
        //}

        //public void StopSmoke()
        //{
        //    if (!smokeFlag) return;
        //    for (int i = 0; i < smokes.Length; i++)
        //    {
        //        smokes[i].Stop();
        //    }
        //    smokeFlag = false;
        //    StopSkid();
        //}
        #endregion

        private void ActivateSkid()
        {
            if (carController.isDrifting && cameraController.isPalying) StartSkid();
            else StopSkid();
        }

        public void StartSkid()
        {
            for (int i = 0; i < skidMarks.Length; i++)
            {
                skidMarks[i].emitting = true;
            }
            SoundMGR.Instance.SoundPlayIfNotPlaying("Skid");
        }

        public void StopSkid()
        {
            for (int i = 0; i < skidMarks.Length; i++)
            {
                skidMarks[i].emitting = false;
            }
            SoundMGR.Instance.SoundStop("Skid"); ;
        }

        private void ActivateBoostEffect(bool boostFlag)
        {
            if (boostFlag)
            {
                StartBoostEffect();
            }
            else
            {
                StopBoostEffect();
            }
        }

        private void StartBoostEffect()
        {
            for (int i = 0; i < boosterEffects.Length; i++)
            {
                StartCoroutine(WaitAndAnimateBoosterEffect(boosterEffects[i].transform, true, 0.75f, 0f));
            }
        }

        private void StopBoostEffect()
        {
            for (int i = 0; i < boosterEffects.Length; i++)
            {
                StartCoroutine(WaitAndAnimateBoosterEffect(boosterEffects[i].transform, false, 0.3f));
            }

            SoundMGR.Instance.SoundStop("Booster");
        }

        private IEnumerator WaitAndAnimateBoosterEffect(Transform boosterTransform, bool toggle, float duration = 0f, float delay = 0f, float increaseAmount = 5f)
        {
            // 대기 시간
            yield return new WaitForSeconds(delay);

            aeroEffect.SetActive(toggle);

            // 초기 스케일을 저장소에서 가져옴
            Vector3 initialScale = initialScales[boosterTransform];

            // 목표 상태 설정
            Vector3 targetScale;
            if (toggle)
            {
                // 크기 증가 시 목표 크기 설정
                targetScale = new Vector3(initialScale.x, initialScale.y, Mathf.Max(boosterTransform.localScale.z, initialScale.z + increaseAmount));
            }
            else
            {
                // 크기 감소 시 원래 크기로 복원
                targetScale = initialScale;
            }

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // 점진적으로 크기 변경
                boosterTransform.localScale = Vector3.Lerp(boosterTransform.localScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 정확히 목표 스케일로 설정
            boosterTransform.localScale = targetScale;
        }


        public void CarResetStart(float duration)
        {
            StartCoroutine(CarResetRoutine(duration));
        }

        IEnumerator CarResetRoutine(float duration)
        {
            if(aeroEffect.activeSelf)
            {
                aeroEffect.SetActive(false);
            }

            // BoosterEffects와 SkidMarks 모두 비활성화
            System.Array.ForEach(boosterEffects, effect => effect.SetActive(false));
            System.Array.ForEach(newSkidMarks, skidMark => skidMark.SetActive(false));

            yield return new WaitForSeconds(duration);

            // BoosterEffects와 SkidMarks 모두 활성화
            System.Array.ForEach(boosterEffects, effect => effect.SetActive(true));
            System.Array.ForEach(newSkidMarks, skidMark => skidMark.SetActive(true));
        }

        public void ActivateBoostEffects()
        {
            System.Array.ForEach(boosterEffects, effect => effect.SetActive(true));
            System.Array.ForEach(newSkidMarks, skidMark => skidMark.SetActive(true));
        }
    }
}
