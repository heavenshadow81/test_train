
using UnityEngine;
using UnityEngine.UI;
public class OptionFunctions : MonoBehaviour
{
    //한영,숫자 바꾸기
    public void ChangeType(Dropdown down)
    {
        ContentsController.Instance.contentsParameter.contents = (BubblePang.ContentsType)down.value;
    }
    //난이도 바꾸기
    public void ChangeDifficulty(Dropdown down)
    {
        ContentsController.Instance.contentsParameter.difficult = (Difficulty)down.value;
    }
    //인원수 설정
    public void ChangePerson(Dropdown down)
    {
        ContentsController.Instance.contentsParameter.person = down.value;
    }
}
