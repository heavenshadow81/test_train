using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FindPotion
{
    [System.Serializable]
    public class Hint
    {
        public Transform[] potionHintTransforms;
    }

    public class PotionManager : MonoBehaviour
    {
        public static PotionManager Instance { get; private set; }  // 싱글톤 인스턴스

        [Header("포션 & 큐브")]
        [SerializeField] private Potion[] potions;
        [SerializeField] private PotionCube cube;

        [Header("힌트")]
        [SerializeField] private Hint[] hints;
        private int resetCount;  // 현재 리셋 완료된 포션 수
        public int activeHintIndex;  // 현재 활성화된 힌트 인덱스
        private int moveCount;  // 현재 활성화된 힌트 인덱스
        private List<Potion> movedPotions = new List<Potion>();  // 이동한 포션들을 저장할 리스트
        private Potion firstHintPotion;  // 첫 번째 힌트에서 선택된 포션을 저장

        [Header("정답")]
        private int answerIdx;
        private Potion answerPotion;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }            
        }

        private void OnDisable()
        {
            if (Instance != null)
            {
                Instance = null;
            }
        }

        public void Init()
        {
            resetCount = 0;
            activeHintIndex = 0;
            moveCount = 0;
            movedPotions.Clear();
            if(answerPotion != null )
            {
                answerPotion.ResetAnswer();
            }

            SetAnswer();
            cube.SetWaterDropEffect();
            StartCoroutine(ShowHintSequence());
        }

        public void SetAnswer()
        {
            answerIdx = Random.Range(0, potions.Length);
            answerPotion = potions[answerIdx];
            answerPotion.SetAnswer();
        }

        private IEnumerator ShowHintSequence()
        {
            //resetCount = 0;  // 리셋 카운트 초기화

            while (activeHintIndex < hints.Length)
            {
                yield return ShowHintWithReset(activeHintIndex);

                activeHintIndex++;
            }

            activeHintIndex = 0;  // 힌트 인덱스 초기화
            GameManager.Instance.ToggleButton();    // 버튼 보이게 처리
            GameManager.Instance.isCheckAnswer = true;  // 정답체크상황 플래그 변경
            GameManager.Instance.EnableTouch();
        }

        private IEnumerator ShowHintWithReset(int hintIdx)
        {
            // 현재 힌트를 표시
            ShowHint(hintIdx);

            // 힌트 인덱스에 따른 현재 힌트에서 이동할 포션 수 가져오기
            int hintPotionCount = hints[hintIdx].potionHintTransforms.Length;

            // 포션이 모두 리셋될 때까지 대기
            yield return new WaitUntil(() => resetCount == hintPotionCount);

            movedPotions.Clear();

            // 리셋 후 텀을 두기 위해 잠시 대기 (여기서 텀을 설정)
            yield return new WaitForSeconds(0.5f);

            resetCount = 0;  // 다음 힌트를 위해 리셋 카운트 초기화
        }

        private void ShowHint(int hintIdx)
        {
            // 정답 포션을 제외한 가상의 포션 리스트 생성
            List<Potion> virtualPotions = new List<Potion>(potions);

            // 첫 번째 힌트인 경우 정답 포션을 제외하고 나머지 중 하나만 포션을 이동
            if (hintIdx == 0)
            {
                virtualPotions.RemoveAt(answerIdx);  // 정답 포션 제거
            }
            else if (hintIdx == 1)
            {
                // 첫 번째 힌트에서 선택된 포션을 참조로 제거
                virtualPotions.Remove(firstHintPotion);
            }

            // 각 힌트에 있는 포지션으로 포션을 이동
            for (int i = 0; i < hints[hintIdx].potionHintTransforms.Length; i++)
            {
                int randNum = Random.Range(0, virtualPotions.Count);

                // 첫 번째 힌트일 때만 firstHintPotion을 설정
                if (hintIdx == 0)
                {
                    firstHintPotion = virtualPotions[randNum];  // 선택된 포션을 참조로 저장
                }

                Potion selectedPotion = virtualPotions[randNum];
                selectedPotion.MovePotion(hints[hintIdx].potionHintTransforms[i], () => OnPotionMoveComplete(selectedPotion));

                movedPotions.Add(selectedPotion);   // 이동한 포션 리스트에 추가
                virtualPotions.RemoveAt(randNum);  // 선택된 포션을 리스트에서 제거
            }
        }

        public void OnPotionMoveComplete(Potion movedPotion)
        {
            moveCount++;  // 리셋된 포션 수 증가

            if (moveCount == movedPotions.Count)
            {
                for(int i = 0;i < movedPotions.Count;i++)
                {
                    movedPotions[i].ActivateWaterDrop();
                    moveCount = 0;
                }
            }
        }

        public void OnPotionResetComplete()
        {
            resetCount++;  // 리셋된 포션 수 증가
        }
    }
}
