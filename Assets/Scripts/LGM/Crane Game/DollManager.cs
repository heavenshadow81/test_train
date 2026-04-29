using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LGM
{
    namespace CraneGame
    {
        public class DollManager : Singleton<DollManager>
        {
            public List<GameObject> dollPrefabs = new List<GameObject>();   // 종류별 인형
            public List<Transform> regenPos = new List<Transform>();    // 인형 생성 위치
            public Image target;
            public List<GameObject> doll = new List<GameObject>();  // 배치된 인형 리스트

            public ZoZoBasePatton<DollManager> zozo;
            public EnumClass stateClass;
            public GameUI gameUI;
            public ScreenProsess screenProsess;

            private void Awake()
            {
                stateClass = new EnumClass();
                #region 공용 스테이트 패턴 

                ActionProcess.Enter_StateListener(Init, null, null, null);

                zozo = new ZoZoBasePatton<DollManager>();
                zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
                #endregion
            }


            private void Init()
            {
                for (int i = 0; i < dollPrefabs.Count; i++)
                {
                    doll.Add(CreateRandomObj(regenPos, dollPrefabs[i]));// 인형 생성
                }
                SetRandomTarget();  // 뽑아야할 인형 랜덤 선택 
            }


            private void Update()
            {
                if (zozo != null) zozo.MGR.Excute(() =>
                {
                    TimeManager.Instance.CurTime();
                    CraneManager.Instance.UpdateLogic();
                });
            }


            // 인형 삭제
            public void DestroyDoll(GameObject _doll)
            {
                doll.Remove(_doll);
                Destroy(_doll);
            }
            // _regenPos 내의 랜덤한 위치에 오브젝트 생성
            public GameObject CreateRandomObj(List<Transform> _regenPos, GameObject _prefab)
            {
                // regenPos 내에서 랜덤한 위치 선택
                int randomPos = Random.Range(0, _regenPos.Count);
                GameObject doll = Instantiate(_prefab, gameObject.transform);
                // 생성한 오브젝트의 위치를 랜덤한 위치로 변경
                doll.transform.localPosition = _regenPos[randomPos].localPosition;
                doll.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                // 랜덤한 위치는 다시 나오지 않도록 삭제 및 공간 삭제
                Destroy(_regenPos[randomPos].gameObject);
                _regenPos.RemoveAt(randomPos);
                return doll;
            }

            public void SetRandomTarget()
            {
                // 인형 중 랜덤으로 뽑아야할 인형 선택
                Sprite sprite = doll[Random.Range(0, doll.Count)].GetSpriteRenderer().sprite;
                target.sprite = sprite;
            }
        }
    }
}