using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  // DOTween 네임스페이스 추가
using System;  // Action을 사용하기 위해 추가

namespace GuessNumber
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private GameObject chestTop;
        [SerializeField] private GameObject orb;

        // chestTop을 현재 로테이션에서 -90도 더해 회전시키는 함수, 딜레이 시간도 매개변수로 받음
        public void RotateChestTop(float delayDuration, Action onComplete = null)
        {
            SoundMGR.Instance.SoundPlay("Open");

            // 현재 chestTop의 로컬 회전 값을 가져옴
            Vector3 currentRotation = chestTop.transform.localEulerAngles;

            // -90도 더한 새로운 회전 값 설정
            Vector3 newRotation = new Vector3(currentRotation.x - 90f, currentRotation.y, currentRotation.z);

            // 두트윈을 이용해 회전 애니메이션 (2초 동안 회전)
            chestTop.transform.DOLocalRotate(newRotation, 2f).OnComplete(() =>
            {
                SpawnOrb(() =>
                {
                    // delayDuration 후에 SpawnOrb와 onComplete 호출
                    DOVirtual.DelayedCall(delayDuration, () =>
                    {
                        onComplete?.Invoke();  // 콜백이 있다면 실행
                    });
                });
            });
        }

        // 오브의 스폰과 애니메이션
        public void SpawnOrb(Action onComplete = null)
        {
            SoundMGR.Instance.SoundPlay("Orb");
            orb.SetActive(true); // 오브 활성화

            // 현재 오브의 로컬 위치 가져오기
            Vector3 currentPosition = orb.transform.localPosition;

            // Y축을 0.04 더한 새로운 위치 설정
            Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y + 0.04f, currentPosition.z);

            // 위치 이동 애니메이션과 스케일 애니메이션을 동시에 실행
            orb.transform.DOLocalMove(newPosition, 1f);
            orb.transform.DOScale(0.05f, 1f).OnComplete(() =>
            {
                onComplete?.Invoke();  // 콜백이 있다면 실행
            });
        }
    }
}
