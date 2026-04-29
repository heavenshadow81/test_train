using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Bax.P0.Client.UnityWorld.BalloonGame
{
    public class BalloonSetter : MonoBehaviour
    {
        //ÃÖ¼Ò°¹¼ö
        public int balloonMinCnt;
        //ÃÖ´ë°¹¼ö
        public int balloonMaxCnt;

        //Balloon ÀÌ µµÂøÇÒ ÀüÃ¼ Transform
        public Transform[] balloonsPositions;


        public spriteData spriteData;

        //balloon °¹¼ö
        [SerializeField] private int balloonCnt;
        //balloon °¹¼ö ÇÁ·ÎÆÛÆ¼
        public int BalloonCnt
        {
            get => balloonCnt;
            set
            {
                balloonCnt = value;
            }
        }

        //¼¯±â
        private int[] shuple()
        {
            int[] rndSort = new int[20];

            for (int i = 0; i < 20; i++) rndSort[i] = i;
            for (int i = 0; i < 10; i++)
            {
                int rest = Random.Range(0, rndSort.Length);
                int dest = Random.Range(0, rndSort.Length);

                int temp = rndSort[rest];
                rndSort[rest] = rndSort[dest];
                rndSort[dest] = temp;
            }
            return rndSort;
        }



        public  async UniTask BalloonPositionSet()
        {
            //Ç³¼± °¹¼ö ·£´ý°ª ¼¼ÆÃ
            BalloonCnt = Random.Range(balloonMinCnt, balloonMaxCnt);
            //1~19 ·£´ýÀ¸·Î ¼¯Àº °ª ÀúÀå
            int[] balloonRndSort = shuple();
            
            SoundMGR.Instance.SoundPlay("22.Ç³¼±³ª¿È");

            for (int i = 0; i < BalloonCnt; i++)
            {
                //Ç³¼± »ý¼º
                var balloon = BalloonMgr.instance.balloonPool.Get();
                
                //Ç³¼± ÃÊ±âÀ§Ä¡ ¼¼ÆÃ 
                balloon.transform.position = new Vector3(0, -10f, 0);
                //Ç³¼± ÀÌ¹ÌÁö ·Îµå
                BalloonMgr.instance.loadSprite.LoadSpriteData("Balloon" + Random.Range(0, 5), balloon.balloonSpRender);

                //Ç³¼± À§Ä¡ TweenÀÌµ¿
                balloon.transform.DOMove(balloonsPositions[balloonRndSort[i]].position, 2f);

                //Ç³¼± »ìÂ¦Èçµé±â loop
                balloon.transform.DOShakeRotation(1f, 10, 3, 30).SetLoops(-1);
            }
        }
    }
}
