using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// ObjectPool 클래스에 꽃게 프리팹 전달 및 꽃게 객체 생성 및 관리 클래스
    /// </summary>
    public class TwoDimensionSandPrintInteraction : MonoBehaviour
    {
        /// <summary>
        /// 꽃게 프리팹
        /// </summary>
        public GameObject crabPrfab;
        /// <summary>
        /// 메모리 풀 클래스
        /// </summary>
        public ObjectPool objectPool;
        /// <summary>
        /// 생성 할 꽃게의 수
        /// </summary>
        public int maximumCrab;

        int textCNt;

        void Awake()
        {
            /*
       Crab _crab = crabPrfab.GetComponent<Crab>();

       ColliderChecker _checker = crabPrfab.GetComponent<ColliderChecker>();
       if (_checker == null)
       {
           _checker =  crabPrfab.AddComponent<ColliderChecker>();
       }
       _checker.callFunc = new EventDelegate(_crab.Touch);
       */
            objectPool.prefab = crabPrfab;

        }

        void OnEnable()
        {
            objectPool.Initialize();
            objectPool.CheckObjectsState();
            StartCoroutine(CheckCrabProcess());
            // boidController.DetectBoid();
        }

        IEnumerator CheckCrabProcess()
        {
            textCNt = 0;
            GameObject go = this.gameObject;
            float _waitTIme = 0;
            do
            {
                if (objectPool.Count < maximumCrab)
                {
                    _waitTIme = Random.Range(1.5f, 2.5f);
                    yield return new WaitForSeconds(_waitTIme);
                    GameObject obj = objectPool.GetObejct();
                    if (obj == null)
                    {
                        Debug.LogError("object null");
                        yield return new WaitForFixedUpdate();
                        continue;
                    }

                    NewCrab _crab = obj.GetComponent<NewCrab>();
                    _crab.obj.SetActive(true);

                    obj.layer = Common.LayerConstants.INTERACTION_OBJECT; //ML.PlaywallKids.Common.LayerConstants.DONT_COLLIDE_WITH_SAMETHING;

                    BoidAgent b = _crab.GetComponent<BoidAgent>();

                    if (b)
                    {
                        b.name = string.Format("Crab{0}", textCNt++);
                        b.type = BoidAgent.EBoidType.FLOCKING_ELEMENT;
                        BoidController.instance.SetBoid(b);
                    }
                }
                else
                {
                    yield return new WaitForFixedUpdate();
                }

            } while (go.activeInHierarchy);
        }
    }
}