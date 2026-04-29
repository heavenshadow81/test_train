#define USING_FRUIT_PROTRAIT

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UserType;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 과일 객체 생성 하는 클래스
    /// </summary>
    [RequireComponent(typeof(TwoDimensionInteractionFruitPanel))]
    public class TwoDimensionFruitManufacturer : MonoBehaviour
    {
        #region PUBLIC_VARIABLES
        /// <summary>
        /// 과일 객체 프리팹
        /// </summary>
        public GameObject fruitPrefab;
        /// <summary>
        /// 생성 개수
        /// </summary>
        [Range(1, 50)]
        public int numFruit;
        /// <summary>
        /// 과일 이미지 배열
        /// </summary>
        [HideInInspector]
        public Texture2D[] imgFruits;
        #endregion PUBLIC_VARIABLES

        #region PRIVATE_VARIABLES
        Camera cam;
        GameObject obj;
        /// <summary>
        /// 현재 동작 중인 코루틴 참조
        /// </summary>
        Coroutine coroutineGenerate;
        TwoDimensionInteractionFruitPanel fruitGround;
        CObjectList<FruitObject> fruitList;
        /// <summary>
        /// 과일 이미지 크기
        /// </summary>
        int[] fruitSizeArr = new int[] { 180, 200, 160, 140, 250 };
        #endregion PRIVATE_VARIABLES

        public static int numOfEnableFruits { get; set; }

        /// <summary>
        /// 초기화
        /// </summary>
        void Awake()
        {
            numOfEnableFruits = 0;
            fruitGround = GetComponent<TwoDimensionInteractionFruitPanel>();
            imgFruits = fruitGround.ImgFruits;
            obj = this.gameObject;
        }

        void OnEnable()
        {
            if (!cam) cam = UICamera.list[0].cachedCamera;

            // 메모리풀 초기화
            fruitList = new CObjectList<FruitObject>(
                0, () =>
                {
                    return NGUITools.AddChild(obj, fruitPrefab).GetComponent<FruitObject>();
                },
                    (FruitObject _fruit) =>
                    {
                        if (_fruit == null) return false;
                        return !_fruit.gameObject.activeInHierarchy;
                    }
                );

            coroutineGenerate = StartCoroutine(FruitManufactureProcess());
        }

        /// <summary>
        /// 과일 생성 코루틴
        /// </summary>
        /// <returns></returns>
        IEnumerator FruitManufactureProcess()
        {            
            numOfEnableFruits = 0;
            yield return new WaitForSeconds(0.1f);
            int len = fruitGround.ImgFruitArr.Length;
            float waitTime = 0;
            while (obj.activeInHierarchy)
            {               
                for (int i = 0; i < len; ++i)
                {
                    if ((Random.Range(0, 2) == 0))
                    {
                        if (numOfEnableFruits <= numFruit) //최대 생성 개수 확인
                        {
#if !USING_FRUIT_PROTRAIT
                        if (UIFruitTexture.curImage != null)
                        {
#endif
                            FruitObject fruit = fruitList.GetObject();

                            ++numOfEnableFruits;
                            fruit.obj.SetActive(false);

#if !USING_FRUIT_PROTRAIT
                            fruit.kindOfFruit = (int)TwoDimensionFruitPrint.eFruitType;
#else
                            fruit.kindOfFruit = Random.Range(0, imgFruits.Length); //과일 종류 설정
#endif
                            yield return new WaitForSeconds(0.2f);

                            UITexture img = fruit.img;
                            do
                            {
#if !USING_FRUIT_PROTRAIT
                           img.mainTexture = UIFruitTexture.curImage;
#else
                                img.mainTexture = imgFruits[fruit.kindOfFruit]; //과일 텍스쳐 설정
#endif
                                if (img.mainTexture == null) yield return new WaitForEndOfFrame();
                            } while (img.mainTexture == null);
#if !USING_FRUIT_PROTRAIT
                            int _size = Random.Range(150, 250);
#else
                            int _size = fruitSizeArr[fruit.kindOfFruit];
#endif
                            // 텍스쳐 사이즈 설정
                            fruit.img.width = _size;
                            fruit.img.height = _size;
                            fruit.transform.localPosition = fruitGround.ImgFruitArr[i].cachedTransform.localPosition; // 생성 위치
                            fruit.obj.SetActive(true);
                            fruit.Jump();
#if !USING_FRUIT_PROTRAIT
                        }
                        else
                        {
                            break;
                        }
#endif
                        }
                        else
                        { yield return new WaitForSeconds(0.5f); }
                    }
                }

                waitTime = Random.Range(2.5f, 3f);

                yield return new WaitForSeconds(waitTime);
            }
        }

        void OnDisable()
        {
            if (coroutineGenerate != null)
            { StopCoroutine(coroutineGenerate); }

            for (int i = 0, len = fruitList.count; i < len; ++i)
                Destroy(fruitList.GetObject(i).obj);

            fruitList.Destroy();
            fruitList = null;
        }
    }
    /*
                               float x = UtilityScript.fWidth * Random.Range(-0.45f, 0.45f);
                               float y = UtilityScript.fHeight * Random.Range(-0.1f, 0.2f);

                               fruit.bezier.target = new Vector2(x, y);
                               fruit.bezier.usePosition = false;
                               fruit.bezier.wayPoint0 = new Vector3(Random.Range(-10f, 10f), 10f, 0);
                               fruit.bezier.disableType = BezierMove.EType.STOP;
                               fruit.bezier.fSpeed = Random.Range(0.65f, 0.75f);
                               */
}