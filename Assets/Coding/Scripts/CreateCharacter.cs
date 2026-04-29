using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCharacter : MonoBehaviour
{
    [Tooltip("생성 될 부모 Transform")]
    [SerializeField]
    Transform createTr;

    [Tooltip("메인카메라")]
    [SerializeField]
    Camera mainCamera;

    [Tooltip("플레이어 캐릭터 오브젝트")]
    public GameObject[] playerCharacter;

    [Tooltip("플레이어 번호")]
    public int playerNumber;
    // 캐릭터 오브젝트 만드는 메소드
    public void CreateCharacterObject(int characterNum)
    {
        // 랜덤 버튼으로 호출 했을 경우
        if (characterNum == 100)
        {
            characterNum = Random.Range(0, playerCharacter.Length);
        }
        // 캐릭터 전체 비활성화
        for (int i = 0; i < playerCharacter.Length; i++)
        {
            playerCharacter[i].SetActive(false);
        }
        // 선택한 캐릭터 활성화
        playerCharacter[characterNum].SetActive(true);
        playerCharacter[characterNum].transform.localPosition = Vector3.zero;

        // 카메라 주시
        var mainCam = Camera.main;
        var camPos = playerCharacter[characterNum].transform.InverseTransformPoint(mainCam.transform.position);
        var targetPos = playerCharacter[characterNum].transform.localPosition;
        var angle = Mathf.Atan(camPos.x - targetPos.x) * (180f / 3.141592f);
        var origin = playerCharacter[characterNum].transform.localEulerAngles;
        playerCharacter[characterNum].transform.localEulerAngles = new Vector3(origin.x, angle - 180, origin.z);

        // 플레이어 캐릭터 저장
        CharacterSourceContainer.Instance.playerObject[playerNumber] = playerCharacter[characterNum];

        // 캐릭터 Body 랜덤 매테리얼
        int ran_Body = Random.Range(0, CharacterSourceContainer.Instance.characters[characterNum].materials.Length);
        // 캐릭터 오브젝트 Body Renderer
        Renderer bodyRenderer = playerCharacter[characterNum].transform.GetChild(1).transform.GetChild(0).GetComponent<Renderer>();
        SkinnedMeshRenderer bodyMesh = bodyRenderer.GetComponent<SkinnedMeshRenderer>();
        // 캐릭터 오브젝트 Body Material 변경
        bodyRenderer.sharedMaterial = CharacterSourceContainer.Instance.characters[characterNum].materials[ran_Body];

        // 캐릭터 Face 랜덤 매테리얼
        int ran_Face = Random.Range(0, CharacterSourceContainer.Instance.FaceMaterials.Length);
        // 캐릭터 프래펩 하위에 존재하는 Face01 오브젝트 Renderer
        Renderer faceRenderer = playerCharacter[characterNum].transform.GetChild(1).transform.GetChild(1).GetComponent<Renderer>();
        faceRenderer.sharedMaterial = CharacterSourceContainer.Instance.FaceMaterials[ran_Face];
    }
}
