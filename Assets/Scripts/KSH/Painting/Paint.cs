using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : MonoBehaviour
{
    //스프라이트 랜더
    private SpriteRenderer spriteRenderer;

    //입력될 컬러값
    public Color paintColor;

    private void Awake()
    {
        //스프라이트 컴포넌트 찾기
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    //터치나 클릭 입력
    public async void Down()
    {
        //색상 저장
        PaintMGR.Instance.PaintColor = paintColor;
        //랜더러 컬러 회색
        spriteRenderer.color = Color.gray;
        //사운드 play
        SoundMGR.Instance.SoundPlay("21.색상변경");
        //0.1초 뒤
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.1f));
        Up();
    }

    public void Up()
    {
        //색상 흰색으로 
        spriteRenderer.color = Color.white;
    }

}
