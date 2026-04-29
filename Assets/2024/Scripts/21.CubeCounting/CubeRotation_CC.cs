using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CubeRotation_CC : MonoBehaviour
{
    [SerializeField] GameObject cube;
    [SerializeField] Button[] buttons = null;

    // 오른쪽으로 30도 회전
    public void Onclick_Right()
    {
        SetButtonsEnable(false);
        SoundMGR.Instance.SoundPlay("PlayGround_Turn");
        // 현재 회전 값에 30도 추가
        cube.transform.DORotate(new Vector3(0, cube.transform.rotation.eulerAngles.y - 90, 0), 0.5f, RotateMode.Fast)
            .SetEase(Ease.Linear).OnComplete(() => SetButtonsEnable(true));
    }

    // 왼쪽으로 30도 회전
    public void Onclick_Left()
    {
        SetButtonsEnable(false);
        SoundMGR.Instance.SoundPlay("PlayGround_Turn");
        // 현재 회전 값에 -30도 추가
        cube.transform.DORotate(new Vector3(0, cube.transform.rotation.eulerAngles.y + 90, 0), 0.5f, RotateMode.Fast)
            .SetEase(Ease.Linear).OnComplete(() => SetButtonsEnable(true));
    }

    public void SetButtonsEnable(bool enable)
    {
        foreach (Button button in buttons)
        {
            button.enabled = enable;
        }
    }
}
