using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CrushCatsle
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField] GameObject fireEffect;
        [SerializeField] GameObject cannonHead;
        [SerializeField] Transform shellPos;
        float rotationDuration = 0.5f; // 회전 애니메이션 지속 시간

        public void TurnCannon(Vector3 targetPos)
        {
            // 대포 본체가 y축만 회전하도록 설정
            Vector3 directionToTarget = (transform.position - targetPos).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // y축 회전만을 사용
            Quaternion yOnlyRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            // 대포가 바라보는 방향을 레이로 시각화
            Debug.DrawRay(transform.position, directionToTarget * 10, Color.blue, 10f);

            // DOTween으로 대포 본체 회전
            transform
                .DORotateQuaternion(yOnlyRotation, rotationDuration)
                .OnComplete(() => TurnCannonHead(targetPos)); // 본체 회전 완료 후 대포 헤드 회전 시작
        }

        private void TurnCannonHead(Vector3 targetPos)
        {
            // 대포 헤드가 x축만 회전하도록 설정
            Vector3 headDirectionToTarget = (cannonHead.transform.position - targetPos).normalized;
            Quaternion headTargetRotation = Quaternion.LookRotation(headDirectionToTarget);

            // x축 회전만을 사용
            Quaternion xOnlyRotation = Quaternion.Euler(headTargetRotation.eulerAngles.x, 0, 0);

            // DOTween으로 대포 헤드 회전
            cannonHead.transform
                .DOLocalRotateQuaternion(xOnlyRotation, rotationDuration)
                .OnComplete(() => FireCannon(targetPos)); // 대포 헤드 회전 완료 후 대포 발사
        }


        private void FireCannon(Vector3 targetPos)
        {
            SoundMGR.Instance.SoundPlay("Fire");
            Shell fireShell = ObjectPooler.SpawnFromPool<Shell>("Shell", shellPos.position, Quaternion.identity);
            fireShell.FireTowards(targetPos);

            // 발사 효과 활성화
            fireEffect.SetActive(true);

            // 1초 후에 발사 효과 비활성화 및 장전 완료 신호 보내기
            fireEffect.transform.DOScale(1, 1f)
                .OnComplete(() =>
                {
                    fireEffect.SetActive(false);
                });

            SoundMGR.Instance.SoundPlay("Cannon");
        }
    }
}
