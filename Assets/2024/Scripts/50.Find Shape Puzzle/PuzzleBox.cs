using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace FindShapePuzzle
{
    public class PuzzleBox : MonoBehaviour
    {
        [SerializeField] private PuzzleManager puzzleManager;
        [SerializeField] private Transform underDeskTransform;
        [SerializeField] Image fadeImage;

        public void BoxAnim()
        {
            Vector3 targetRotation = new Vector3(90f, 0f, 0f);

            transform.DOLocalRotate(targetRotation, 2f).SetEase(Ease.OutBounce).SetUpdate(UpdateType.Fixed)
                .OnUpdate(() =>
                {
                    // 회전 각도를 확인하여 특정 각도에서 퍼즐을 던지기
                    if (transform.localEulerAngles.x >= 78f && transform.localEulerAngles.x < 83f)
                    {
                        ThrowPuzzle();
                    }
                })
                .OnComplete(() =>
                {
                    SoundMGR.Instance.SoundPlay("PuzzleFalling");

                    DOVirtual.DelayedCall(5f, () =>
                    {
                        fadeImage.DOFade(1f, 0.5f).OnComplete(() =>
                        {
                            GameManager.Instance.cam.MoveCam(1, 0f, () => GameManager.Instance.magicCircles.SetActive(true));
                            fadeImage.DOFade(0f, 0.5f).OnComplete(() =>
                            {
                                GameManager.Instance.SetPuzzleQuiz();
                            });
                        });
                    });
                });
        }


        private void ThrowPuzzle()
        {
            for (int i = 0; i < puzzleManager.Puzzles.Length; i++)
            {
                Rigidbody rb = puzzleManager.Puzzles[i].gameObject.GetComponent<Rigidbody>();

                // 회전 후 박스의 새로운 윗방향 (로컬 Y축)으로 힘을 가함
                Vector3 randomDirection = transform.up + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));  // 상자의 윗방향에 약간의 랜덤 방향 추가
                rb.AddForce(randomDirection * Random.Range(2.3f, 2.7f), ForceMode.Impulse);  // 새로운 상자의 윗방향으로 랜덤한 힘을 가함
            }
        }

        public void MoveBoxUnderDesk()
        {
            transform.position = underDeskTransform.position;

            transform.rotation = underDeskTransform.rotation;
        }
    }
}
