using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 코스튬 초기화 및 랜덤 적용
public class CostumeSelect : MonoBehaviour
{
    [Tooltip("캐릭터 생성 컴포넌트")]
    CreateCharacter createCharacter;

    [Tooltip("플레이어 번호")]
    int playerIndex;

    [SerializeField]
    [Tooltip("현재 플레이어가 선택 한 캐릭터 오브젝트")]
    GameObject playerCharacter;

    [SerializeField]
    [Tooltip("현재 캐릭터 - ChestObject")]
    GameObject characterChestObject;

    [SerializeField]
    [Tooltip("chestCostume 위치")]
    GameObject chestCostume;

    [SerializeField]
    [Tooltip("headCostume 위치")]
    GameObject headCostume;

    private void Awake()
    {
        // CreateCharacter Component 들고오기
        createCharacter = transform.GetComponent<CreateCharacter>();

        // 플레이어 번호 들고오기
        playerIndex = createCharacter.playerNumber;
    }

    // 기초 셋팅
    public void DefaultSetting()
    {
        // 현재 선택 된 캐릭터 오브젝트 들고오기
        playerCharacter = CharacterSourceContainer.Instance.playerObject[playerIndex];

        // 선택 된 캐릭터 오브젝트 -> Root_M(2) -> Spine1_M(2) -> Chest_M(0)
        characterChestObject = playerCharacter.transform.GetChild(2).GetChild(2).GetChild(0).gameObject;

        ChestCostumeSetting();
        HeadCostumeSetting();
    }

    // 몸, 머리, 손 - 3개 코스튬 한번에 장착
    public void CustumesEquip()
    {
        // 코스튬 초기화
        ChestCostumeSetting();
        HeadCostumeSetting();

        // 코스튬 장착
        ChestCostumeEquip();
        HeadCostumeEquip();
    }

    // 몸통 코스튬 초기화
    public void ChestCostumeSetting()
    {
        // Accessories_locator
        /*
         * ChracterObject -> Root_M(2) -> Spine1_M(2) -> Chest_M(0) -> Accessories_locator(0)
         */

        // Chest 코스튬을 가지고 있는 부모 오브젝트 가져오기
        chestCostume = characterChestObject.transform.GetChild(0).gameObject;

        // Chest 코스튬 모두 비활성화
        for (int i = 0; i < chestCostume.transform.childCount; i++)
        {
            chestCostume.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // 머리 코스튬 초기화
    public void HeadCostumeSetting()
    {
        // Head_Accessories_locator
        /*
         * ChracterObject -> Root_M(2) -> Spine1_M(2) -> Chest_M(0) -> Neck_M(1) -> Head_M(0) -> Head_Accessories_Locator(0)
         */

        // Head 코스튬을 가지고 있는 부모 오브젝트 가져오기
        headCostume = characterChestObject.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;

        // 머리 코스튬 모두 비활성화
        for (int i = 0; i < headCostume.transform.childCount; i++)
        {
            headCostume.transform.GetChild(i).gameObject.SetActive(false);
        }

    }

    // 몸통 코스튬 랜덤 착용
    public void ChestCostumeEquip()
    {
        int ran = Random.Range(0, chestCostume.transform.childCount);

        chestCostume.transform.GetChild(ran).gameObject.SetActive(true);
    }

    // 머리 코스튬 랜덤 착용
    public void HeadCostumeEquip()
    {
        int ran = Random.Range(0, headCostume.transform.childCount);

        headCostume.transform.GetChild(ran).gameObject.SetActive(true);
    }
}
