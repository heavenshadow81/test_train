using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 씬에 있는 버튼 클릭 이벤트
public class CharacterSceneButton : MonoBehaviour
{
    /*
     * int characterNumber;
     * 
     * 1 ~ 캐릭터 종류 = 선택 할 수 있는 캐릭터 갯수
     * 99 = 선택 완료 및 기타 버튼 인덱스
     * 100 = 랜덤 캐릭터 선택 버튼 인덱스
     * 
     */

    [Tooltip("캐릭터 프래펩 번호")]
    [SerializeField]
    int characterNumber;

    [Tooltip("Ready 버튼")]
    [SerializeField]
    GameObject readyBtn;

    [Tooltip("Start 버튼")]
    [SerializeField]
    GameObject startBtn;

    CreateCharacter characterComponent;

    [Tooltip("TTS Sound Play")]
    [SerializeField]
    AudioSource sound;

    private void Awake()
    {
        characterComponent = transform.parent.parent.GetComponent<CreateCharacter>();
    }

    // 캐릭터 만드는 메소드 호출
    public void CreateCharacter()
    {
        characterComponent.CreateCharacterObject(characterNumber);
    }
    
    // 캐릭터 선택 완료 버튼 클릭 이벤트 코루틴 호출
    public void SelectOK()
    {
        StartCoroutine(OKButtonAction());
    }

    // 캐릭터 선택 완료 버튼 클릭 이벤트 구현 메소드
    IEnumerator OKButtonAction()
    {
        // 준비 완료 누른 인원수 추가
        TotalParameter.Instance.readyCount++;

        // 준비 완료를 플레이 인원수 만큼 누르지 않았다면
        if (TotalParameter.Instance.persons > TotalParameter.Instance.readyCount)
        {
            print(TotalParameter.Instance.persons - TotalParameter.Instance.readyCount + "명 더 눌러야해요");

            yield break;
        }
        // 플레이 인원수 모두 선택 완료를 눌렀다면
        else if (TotalParameter.Instance.persons == TotalParameter.Instance.readyCount)
        {
            print("다눌렀어요");

            yield return new WaitForSeconds(0.5f);

            // Ready 버튼 보여주기
            readyBtn.SetActive(true);

            // Ready TTS 재생
            sound.PlayOneShot(CharacterSourceContainer.Instance.buttonTTS[0]);

            // TTS 재생 끝나고 2초 후
            yield return new WaitForSeconds(2.5f);

            readyBtn.SetActive(false);

            // Start 버튼 보여주기  
            startBtn.SetActive(true);

            // Start TTS 재생
            sound.PlayOneShot(CharacterSourceContainer.Instance.buttonTTS[1]);

            // TTS 재생 끝나고 2초 후
            yield return new WaitForSeconds(2.5f);

            // 게임 오브젝트 부모 변경 및 위치 변경
            for(int i = 0; i < TotalParameter.Instance.persons; i++)
            {
                CharacterSourceContainer.Instance.playerObject[i].transform.parent = CharacterSourceContainer.Instance.playerStartPosition[i].transform;
                CharacterSourceContainer.Instance.playerObject[i].transform.position = CharacterSourceContainer.Instance.playerStartPosition[i].transform.position;

                CharacterSourceContainer.Instance.playerObject[i].transform.localPosition = new Vector3(0, 0.2f, 0);
                CharacterSourceContainer.Instance.playerObject[i].transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                CharacterSourceContainer.Instance.playerObject[i].transform.localScale = Vector3.one * 2.3f;
                CharacterSourceContainer.Instance.playerObject[i].AddComponent<CharacterMove>();
                CharacterSourceContainer.Instance.playerObject[i].GetComponent<CharacterMove>().playerNumber = i;
            }

            // GameScene Load
            ChangeScene.Instance.GameSceneLoad();
            print("GameScene Load");

            yield break;
        }

    }
}
