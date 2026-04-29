using DG.Tweening;
using UnityEngine;

namespace ClimbTheTower
{
    public class RotateTower : MonoBehaviour
    {
        [SerializeField] GameObject tower;
        [SerializeField] Player player;
        [SerializeField] FollowCamera cam;

        private float turnDuration = 0.5f;

        private void OnEnable()
        {
            SoundMGR.Instance.SoundPlay("Tower");

            // Sequence를 사용해 동시에 움직이면서 회전하게 설정
            Sequence onEnableSequence = DOTween.Sequence();

            // Y축으로 떨어지는 애니메이션(2초 동안 y=0으로 이동)
            onEnableSequence.Append(transform.DOMoveY(0, 2f).SetEase(Ease.OutQuad));

            // 동시에 2초 동안 720도 회전 (2바퀴 회전)
            onEnableSequence.Join(transform.DORotate(new Vector3(0, 720, 0), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear));

            onEnableSequence.OnComplete(() =>
            {
                player.gameObject.SetActive(true); // 플레이어 활성화
                cam.FollowCameraOnOff(true); // 카메라 활성화
            });
        }

        // 오른쪽으로 30도 회전
        public void Onclick_Right()
        {
            if (!gameObject.activeInHierarchy) return; // 게임 오브젝트가 비활성화 상태라면 실행하지 않음

            //SoundMGR.Instance.SoundPlay("PlayGround_Turn");
            // 현재 회전 값에 30도 추가
            tower.transform.DORotate(new Vector3(0, tower.transform.rotation.eulerAngles.y + 360 / 8, 0), turnDuration, RotateMode.Fast)
                .SetDelay(player.GetJumpReadyTime())  // 딜레이 추가 (플레이어 점프 준비 시간)
                .SetEase(Ease.Linear);
        }

        // 왼쪽으로 30도 회전
        public void Onclick_Left()
        {
            if (!gameObject.activeInHierarchy) return; // 게임 오브젝트가 비활성화 상태라면 실행하지 않음

            //SoundMGR.Instance.SoundPlay("PlayGround_Turn");
            // 현재 회전 값에 -30도 추가
            tower.transform.DORotate(new Vector3(0, tower.transform.rotation.eulerAngles.y - 360 / 8, 0), turnDuration, RotateMode.Fast)
             .SetDelay(player.GetJumpReadyTime())  // 딜레이 추가 (플레이어 점프 준비 시간)
             .SetEase(Ease.Linear);
        }
    }
}


