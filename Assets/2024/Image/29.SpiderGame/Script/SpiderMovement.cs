using DG.Tweening;
using UnityEngine;

public class SpiderMovement : MonoBehaviour
{
    public float wobbleAngle = 9f; // ¾Á·è¾Á·è °Å¸®°Ô ÇÒ °¢µµ
    public float wobbleDuration = 0.7f; // ÇÑ ¹øÀÇ Èçµé¸² Áö¼Ó ½Ã°£
    //public int wobbleRepeats = 10000; // Èçµé¸² ¹Ýº¹ È½¼ö
    public float wobbleSpeed = 2f; // ¾Á·è°Å¸®´Â ¼Óµµ Á¶Àý

    void Start()
    {
        WobbleSpider();
    }

    void WobbleSpider()
    {
        // ¾Á·è¾Á·è °Å¸®°Ô ÇÏ´Â ¾Ö´Ï¸ÞÀÌ¼Ç
        Sequence wobbleSequence = DOTween.Sequence();


            wobbleSequence.Append(transform.DORotate(new Vector3(0, 0, wobbleAngle), wobbleDuration / (2 * wobbleSpeed)).SetEase(Ease.OutQuad))
                          .Append(transform.DORotate(new Vector3(0, 0, -wobbleAngle), wobbleDuration / (2 * wobbleSpeed)).SetEase(Ease.OutQuad))
                          .Append(transform.DORotate(Vector3.zero, wobbleDuration / (2 * wobbleSpeed)).SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Yoyo));
        

        wobbleSequence.Play();
    }
}