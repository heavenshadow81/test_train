using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Railroad
{
    [System.Serializable]
    public class LevelStages
    {
        public List<GameObject> stages = null;
    }

    public class GameManager : MonoBehaviour
    {
        int level = 0;
        int maxLevel = 3;
        int score = 0;
        int maxScore = 6;
        public int failIndex = 0;
        GameObject currentStage;

        [SerializeField] List<LevelStages> levels = null;
        [SerializeField] Image fade;
        [SerializeField] GameObject winUI;
        [SerializeField] GameObject topCamera;
        [SerializeField] TextMeshProUGUI scoreText;

        private void OnEnable()
        {
            if (levels != null && levels.Count > 0 && levels[level].stages.Count > 0)
            {
                // 첫 번째 레벨의 스테이지 리스트에서 랜덤으로 하나 선택
                int randomIndex = Random.Range(0, levels[level].stages.Count);
                currentStage = levels[level].stages[randomIndex];

                // 선택된 스테이지 활성화
                currentStage.SetActive(true);

                // 리스트에서 해당 스테이지 제거
                RemoveStage(level, randomIndex);
            }
        }

        public void NextStage()
        {
            if (failIndex >= 3) return;

            if (score >= maxScore)
            {
                GameClear();
            }
            else
            {
                // 두 번의 플레이 후 레벨을 증가시키고, 최대 레벨을 넘지 않도록 함
                if (score % 2 == 0 && score != 0 && level < maxLevel - 1)
                {
                    level++;
                }

                fade.DOFade(0, 1);
                currentStage.SetActive(false);

                int randomIndex = Random.Range(0, levels[level].stages.Count);
                currentStage = levels[level].stages[randomIndex];

                // 선택된 스테이지 활성화
                currentStage.SetActive(true);

                // 리스트에서 해당 스테이지 제거
                RemoveStage(level, randomIndex);
            }
        }

        public void SetScore()
        {
            score++;
            scoreText.text = $"{score}  /  {maxScore}";
        }

        // 게임 클리어 처리 함수
        private void GameClear()
        {
            winUI.SetActive(true);
            SoundMGR.Instance.bgmSource.Stop();
            SoundMGR.Instance.SoundPlay("win");
        }

        private void RemoveStage(int levelIndex, int stageIndex)
        {
            if (levelIndex < levels.Count && stageIndex < levels[levelIndex].stages.Count)
            {
                levels[levelIndex].stages[stageIndex] = null;
                List<GameObject> updatedStages = new List<GameObject>(levels[levelIndex].stages);
                updatedStages.RemoveAt(stageIndex);
                levels[levelIndex].stages = updatedStages;

                UpdateSerializedObject();
            }
        }

        private void UpdateSerializedObject()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            SerializedObject serializedObject = new SerializedObject(this);
            serializedObject.UpdateIfRequiredOrScript();
#endif
        }
    }
}
