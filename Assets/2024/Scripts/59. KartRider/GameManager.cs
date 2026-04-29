using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KartRider
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private CarController carController;
        [SerializeField] private GameObject needle;
        [SerializeField] private TextMeshProUGUI kphText;
        [SerializeField] private TextMeshProUGUI gearNumber;

        [Header("바늘 설정")]
        private float startPosition = 32f; // 바늘 시작 각도
        private float endPosition = -221f; // 바늘 끝 각도
        private float desiredPosition;

        private float smoothRPM; // 부드럽게 보간된 RPM 값

        [SerializeField] DontDestroyObject[] dontDestroyObjects = null;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance != null) Instance = null;
        }

        private void Start()
        {
            FindObjectsWithDontDestroyScript();
        }

        private void FixedUpdate()
        {
            UpdateSpeedText();
            UpdateNeedle();
        }

        // 속도 텍스트 업데이트
        private void UpdateSpeedText()
        {
            // 화면에 표시되는 속도를 실제 속도의 절반으로 표시
            float displayedSpeed = carController.KmH * 0.5f;
            //float displayedSpeed = carController.rb.velocity.magnitude;
            kphText.text = $"{displayedSpeed:0}";
        }

        // RPM과 바늘 회전 업데이트
        private void UpdateNeedle()
        {
            // RPM 값 부드럽게 보간
            smoothRPM = Mathf.Lerp(smoothRPM, carController.engineRPM, Time.deltaTime * 5f);

            // 바늘 회전 계산
            desiredPosition = startPosition - endPosition;
            float temp = smoothRPM / 10000f; // RPM을 0~1 범위로 정규화
            float needleRotation = startPosition - (temp * desiredPosition);

            // 바늘 회전 적용
            needle.transform.eulerAngles = new Vector3(0, 0, needleRotation);
        }

        // 기어 상태 업데이트
        public void ChangeGear()
        {
            if (carController.KmH == 0)
            {
                gearNumber.text = "N"; // 정지 상태에서는 N 표시
                return;
            }
            gearNumber.text = carController.reverse ? "R" : (carController.gearNumber + 1).ToString();
        }

        private void FindObjectsWithDontDestroyScript()
        {
            // 현재 씬에서 모든 게임 오브젝트 가져오기
            dontDestroyObjects = FindObjectsOfType<DontDestroyObject>();
        }

        public void DestroyAllDontDestroyObjects()
        {
            foreach (var obj in dontDestroyObjects)
            {
                obj.DestroyObject();
            }
        }
    }

}
