using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//버튼이 공통으로 가지고 있을 함수
public interface ITouchButton
{
    //버튼 설정
    void SetButton(int order);
    //버튼 활성,비활성
    void Activate(bool state);
}
//개인 UI들..
public interface IPersonal
{
    //그림 순서: 플레이어 배경으로 선택될 오브젝트를 지정하기 위한 숫자
    int ButtonIndex { get; set; }
    //가진 리스트에 추가하기...
    void Add(ITouchButton touchbutton);
    //오브젝트 비활성화
    void Disactive();
    
}
//컨트롤러가 가지고 있을 것
public interface IUIController
{
    //플레이어 추가
    void Add(IPersonal personal);
    //플레이어 제거
    void Remove(IPersonal personal);
}