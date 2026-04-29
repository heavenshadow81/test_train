using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectScene : MonoBehaviour
{
    // 홈버튼 클릭 이벤트 메소드
    public void ChangeScene()
    {
        // 홈화면으로 이동시 default 값으로 초기화
        DinoSceneOptions.RetrySaveParameter(0, 0, 0, 0);

        // 홈화면으로 이동
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nameof(ContentsStoreItemType.Mode.BubblePang));
    }
}
