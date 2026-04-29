using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RenderNArrowType
{
    public SpriteRenderer ArrowRender;
    public DisplayArrowTYPE arrowTYPE;

    public RenderNArrowType(SpriteRenderer _ArrowRender, DisplayArrowTYPE _arrowTYPE)
    { 
        this.ArrowRender = _ArrowRender;
        this.arrowTYPE = _arrowTYPE;
    }
}

public class Electronicdisplay : MonoBehaviour
{
    public Queue<RenderNArrowType> displayQueue = new Queue<RenderNArrowType>();

    //화살표 방향 오브젝트 6개 
    public SpriteRenderer[] ArrowRenders;

    //보여지는 칸 5칸 + 숨겨져있는칸 1
    public Vector3[] arrowPos = new Vector3[6];
    

    public async void QueueArrowAdd()
    {
        for (int i = 0 ; i < 6 ; i++) 
        {
            //0~3 랜덤값
            int rnd = Random.Range(0, 4);
            //해당방향 이미지 로딩 
            await setImages(((DisplayArrowTYPE)rnd).ToString(), ArrowRenders[i]);
            //로딩완료된 이미지 컬러 흑백
            ArrowRenders[i].color = Color.gray;
            //방향과 랜더러 저장
            RenderNArrowType renderType = new RenderNArrowType(ArrowRenders[i], (DisplayArrowTYPE)rnd);
            //큐 저장
            displayQueue.Enqueue(renderType);
        }
        //맞춰야될 첫번째 화살표 컬러 흰색으로 변경
        displayQueue.Peek().ArrowRender.color = Color.white;

        //큐를 리스트로 
        var arr = MerryGoMGR.Instance.display.displayQueue.ToList();
        //화살표 오브젝트 미리 세팅해둔 위치로 저장
        for (int i = 0; i < arrowPos.Length; i++)
        {
            arrowPos[i] = arr[i].ArrowRender.transform.position;
        }
    }

    //이미지 로드
    public async UniTask setImages(string spriteName , SpriteRenderer render)
    { 
        await MerryGoMGR.Instance.loadSprite.LoadSpriteData(spriteName , render);
    }
}