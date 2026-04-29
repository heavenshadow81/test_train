using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random;

// 캐릭터 이동 스크립트
public class CharacterMove : MonoBehaviour
{
    Animator ani;
    CapsuleCollider capsuleCollider;

    public TotalParameter.Arrow[] arrow;

    // 선택한 화살표에서 현재 실행하고 있는 화살표는 몇번째?
    int arrowCount;

    // 현재 선택한 화살표 개수
    int arrowTotalCount;

    // 이동 횟수
    public int moveCount;

    // 이동 가능 여부
    bool isMove;
    public bool IsMoving { get; private set; }

    // 방향 전환 가능 여부
    public bool isRotate;

    // 목표 타겟 Position
    Vector3 target;

    // 목표 타겟 Lotation
    Vector3 lookTarget;

    // 목표 타겟 Transform
    public Transform targetTr;

    // 캐릭터 방향 전환 속도
    [SerializeField]
    float rotateSpeed = 5.0f;

    public SelectedButtons selectedButtons;

    // 현재 플레이어 번호
    public int playerNumber;

    // 현재 플레이어 Row, Column
    public int rowNum, columnNum;

    // 시작 Row, Column
    public int startRow, startColumn;

    // 마지막 Row, Column
    int lastRow, lastColumn;

    // 손 코스튬 획득 여부
    bool haveCostume;

    [SerializeField]
    [Tooltip("elbowCostume 위치")]
    GameObject elbowCostume;

    GameObject characterChestObject;
    private bool isPrivateCameraActivated;

    void Start()
    {
        ani = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        isMove = false;
        IsMoving = false;
        haveCostume = false;
        isRotate = false;

        // 플레이 인원수 = 2명
        if (TotalParameter.Instance.persons == 2)
        {
            // player 0 이면
            if (playerNumber == 0)
            {
                rowNum = startRow = 0;
                columnNum = startColumn = 1;
            }
            // player 1 이면
            else if (playerNumber == 1)
            {
                rowNum = startRow = 0;
                columnNum = startColumn = CharacterSourceContainer.Instance.totalColumn - 2;
            }
        }
        // 플레이 인원수 = 1명
        else if (TotalParameter.Instance.persons == 1)
        {
            // player 1 이면
            if (playerNumber == 0)
            {
                // 난이도가 쉬움
                if (ContentsOptions.GetDifficult() == Difficult.Easy)
                {
                    rowNum = startRow = 0;
                    columnNum = startColumn = 3;
                }
                // 난이도가 보통
                else if (ContentsOptions.GetDifficult() == Difficult.Normal)
                {
                    rowNum = startRow = 0;
                    columnNum = startColumn = 4;
                }
            }
        }
        // 캐릭터 코스튬이 위치하는 부모 오브젝트
        characterChestObject = transform.GetChild(2).GetChild(2).GetChild(0).gameObject;

        isPrivateCameraActivated = false;
        Coding.UIController.Instance.PrivateCameraStateChangedEvent += OnPrivateCameraStateChangedEvent;
    }

    private void FixedUpdate()
    {
        // 이동 가능하면
        if (isMove)
        {
            transform.position = Vector3.Lerp(transform.position, target, 0.1f);
        }
        // 방향 전환 가능하면
        if (isRotate)
        {
            Vector3 dir = targetTr.position - transform.position;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotateSpeed * Time.deltaTime);
        }
    }

    // 캐릭터 이동 외부 호출용 메소드
    public void CharacterMoving(GameObject obj, TotalParameter.Arrow[] arrows, int count)
    {
        // 선택 된 arrow 값 할당
        arrow = arrows;

        // 선택 된 arrow 갯수 할당
        arrowTotalCount = count;

        // arrow Index 초기화
        arrowCount = 0;

        // 이동 시작 호출
        StartCoroutine(IPlayerMoving(obj));
    }

    // 이동 코루틴 메소드
    IEnumerator IPlayerMoving(GameObject obj)
    {
        IsMoving = true;

        Vector3 vec;

        // 캐릭터 이동시 카메라 뷰 켜기
        SetActivePrivateCamera(true);

        // arrow 값에 따라 이동 position 할당
        switch (arrow[arrowCount])
        {
            // 선택 화살표 없음
            case TotalParameter.Arrow.none:
                vec = new Vector3(0.0f, 0.0f, 0.0f);

                // 코루틴 멈추기
                StopAllCoroutines();
                //이동 화면 끄기 추가!!!
                SetActivePrivateCamera(false);

                // 이동 끝나고 호출하는 메소드
                selectedButtons.CharacterMoveEnd();
                break;

            // 선택 화살표 오른쪽
            case TotalParameter.Arrow.right:
                // 마지막 Column 할당
                lastColumn = columnNum + 1;

                // 이동 하려는 Column이 맵의 최대 Column 보다 같거나 크면
                if (lastColumn >= CharacterSourceContainer.Instance.totalColumn)
                {
                    // 이동하지 않도록하고, 카메라 뷰를 끄고 멈춘다.
                    selectedButtons.CharacterMoveEnd();
                    SetActivePrivateCamera(false);

                    yield break;
                }
                // 마지막 Row 할당
                lastRow = rowNum;
                // Colmn과 Row에 따라서 vector3 값 할당
                vec = CharacterSourceContainer.Instance.tiles[rowNum].tilePositions[columnNum + 1];
                // Colmn과 Row에 따라서 transform 할당
                targetTr = CharacterSourceContainer.Instance.tiles[rowNum].tilePrefabs[columnNum + 1].transform;
                // 방향 회전값 할당
                lookTarget = new Vector3(0.0f, 90.0f, 0.0f);

                print("오른쪽으로 갑니다");
                break;
            // 선택 화살표 왼쪽
            case TotalParameter.Arrow.left:
                // 마지막 Column 할당
                lastColumn = columnNum - 1;
                // 이동 하려는 Column이 맵의 최대 Column 보다 작으면
                if (lastColumn < 0)
                {
                    // 이동하지 않도록하고, 카메라 뷰를 끄고 멈춘다.
                    selectedButtons.CharacterMoveEnd();
                    SetActivePrivateCamera(false);

                    yield break;
                }
                // 마지막 Row 할당
                lastRow = rowNum;
                // Colmn과 Row에 따라서 vector3 값 할당
                vec = CharacterSourceContainer.Instance.tiles[rowNum].tilePositions[columnNum - 1];
                // Colmn과 Row에 따라서 transform 할당
                targetTr = CharacterSourceContainer.Instance.tiles[rowNum].tilePrefabs[columnNum - 1].transform;
                // 방향 회전값 할당
                lookTarget = new Vector3(0.0f, -90.0f, 0.0f);

                print("왼쪽으로 갑니다");
                break;

            // 선택 화살표 앞쪽
            case TotalParameter.Arrow.front:
                // 마지막 Row 할당
                lastRow = rowNum + 1;
                // 이동 하려는 Row가 맵의 최대 Row 보다 같거나 크면
                if (lastRow >= CharacterSourceContainer.Instance.totalRow)
                {
                    // 이동하지 않도록하고, 카메라 뷰를 끄고 멈춘다.
                    selectedButtons.CharacterMoveEnd();
                    SetActivePrivateCamera(false);

                    yield break;
                }
                // 마지막 Column 할당
                lastColumn = columnNum;
                // Colmn과 Row에 따라서 vector3 값 할당
                vec = CharacterSourceContainer.Instance.tiles[rowNum + 1].tilePositions[columnNum];
                // Colmn과 Row에 따라서 transform 할당
                targetTr = CharacterSourceContainer.Instance.tiles[rowNum + 1].tilePrefabs[columnNum].transform;
                // 방향 회전값 할당
                lookTarget = new Vector3(0.0f, 0.0f, 0.0f);

                print("앞으로 갑니다");
                break;
            // 선택 화살표 뒤쪽
            case TotalParameter.Arrow.back:
                // 마지막 Row 할당
                lastRow = rowNum - 1;
                // 이동 하려는 Row가 맵의 최소 Row 보다 작으면
                if (lastRow < 0)
                {
                    // 이동하지 않도록하고, 카메라 뷰를 끄고 멈춘다.
                    selectedButtons.CharacterMoveEnd();
                    SetActivePrivateCamera(false);
                    yield break;
                }
                // 마지막 Column 할당
                lastColumn = columnNum;
                // Colmn과 Row에 따라서 vector3 값 할당
                vec = CharacterSourceContainer.Instance.tiles[rowNum - 1].tilePositions[columnNum];
                // Colmn과 Row에 따라서 transform 할당
                targetTr = CharacterSourceContainer.Instance.tiles[rowNum - 1].tilePrefabs[columnNum].transform;
                // 방향 회전값 할당
                lookTarget = new Vector3(0.0f, 180.0f, 0.0f);

                print("뒤로 갑니다");
                break;

            default:
                lastRow = rowNum - 1;
                if (lastRow >= CharacterSourceContainer.Instance.totalRow)
                {
                    selectedButtons.CharacterMoveEnd();
                    SetActivePrivateCamera(false);
                    yield break;
                }
                lastColumn = columnNum;
                vec = CharacterSourceContainer.Instance.tiles[rowNum - 1].tilePositions[columnNum];
                targetTr = CharacterSourceContainer.Instance.tiles[rowNum - 1].tilePrefabs[columnNum].transform;
                lookTarget = new Vector3(0.0f, 0.0f, 0.0f);

                print("넌 모야? 어디로가?");
                break;
        }

        yield return new WaitForSeconds(0.5f);

        lookTarget = new Vector3(0.0f, vec.y, 0.0f);//vec - transform.position;

        // 방향 회전 가능
        isRotate = true;

        yield return new WaitForSeconds(1.0f);

        // 방향 회전 불가능
        isRotate = false;

        // Jump 애니메이션 실행
        ani.SetInteger("animation", 9);

        // 목표 position 값에 이동값을 더해준다.
        target = new Vector3(vec.x, transform.position.y, vec.z);

        // 이동 시작
        isMove = true;

        //추가된 부분
        Coding.ContentsController.Instance.SoundEffect(0);
        SetActivePrivateCamera(isMove);

        yield return new WaitForSeconds(1.5f);

        // 이동 정지
        isMove = false;

        // arrow 값에 따라 Row, Column 할당
        switch (arrow[arrowCount])
        {
            // 방향 없음
            case TotalParameter.Arrow.none:
                vec = new Vector3(0.0f, 0.0f, 0.0f);

                print("아무데도 안가");
                StopAllCoroutines();
                //이동 화면 끄기 추가!!!
                SetActivePrivateCamera(false);
                selectedButtons.CharacterMoveEnd();
                break;

            // 방향 오른쪽
            case TotalParameter.Arrow.right:
                // 캐릭터 현재 Column값 변경
                columnNum = columnNum + 1;

                print("Column + 1");
                break;

            // 방향 왼쪽
            case TotalParameter.Arrow.left:
                // 캐릭터 현재 Column값 변경
                columnNum = columnNum - 1;

                print("Column - 1");
                break;

            // 방향 앞쪽
            case TotalParameter.Arrow.front:
                // 캐릭터 현재 Row값 변경
                rowNum = rowNum + 1;

                print("Row + 1");
                break;

            // 방향 뒤쪽
            case TotalParameter.Arrow.back:
                // 캐릭터 현재 Row값 변경
                rowNum = rowNum - 1;

                print("Row - 1");
                break;

            default:
                rowNum = rowNum - 1;

                print("넌 모야? 어디로가?");
                break;
        }

        yield return new WaitForSeconds(1.5f);

        // 이동 횟수 변경
        moveCount++;
        Coding.UIController.Instance.moveCountText[playerNumber].text = moveCount.ToString();

        // Idle 애니메이션 실행
        ani.SetInteger("animation", 1);

        // 이동 해야 할 arrow 가 더 있으면 다시 코루틴 호출
        if (arrowTotalCount - 1 > 0)
        {
            // 이동 해야 할 arrow 값 차감
            arrowTotalCount--;

            // arrowIndex 증가
            arrowCount++;

            StartCoroutine(IPlayerMoving(obj));
        }
        else
        {
            //추가된 부분
            SetActivePrivateCamera(isMove);

            // 이동 끝
            StartCoroutine(IMoveEnd(obj));
        }
    }

    private void SetActivePrivateCamera(bool isActive)
    {
        isPrivateCameraActivated = isActive;

        var camIndex = transform.parent.GetSiblingIndex();
        var cam = Coding.UIController.Instance.movingCharacterView[camIndex].gameObject;
        if (!Coding.UIController.Instance.IsPrivateCameraConnected)
        {
            cam.SetActive(false);
            return;
        }
        cam.SetActive(isActive);
    }

    private void OnPrivateCameraStateChangedEvent(object sender, EventArgs eventArgs)
    {
        SetActivePrivateCamera(isPrivateCameraActivated);
    }

    // 이동 끝나고 호출 할 메소드
    IEnumerator IMoveEnd(GameObject obj)
    {
        yield return new WaitForSeconds(2.0f);

        //Idle 애니매이션 실행
        ani.SetInteger("animation", 1);
        capsuleCollider.enabled = true;

        // 위치값이 틀어졌을 경우를 위해 캐릭터가 있어야할 position값을 재할당
        Vector3 vec = CharacterSourceContainer.Instance.tiles[rowNum].tilePositions[columnNum];
        transform.position = new Vector3(vec.x, transform.position.y, vec.z);

        // 캐릭터 이동 카메라 뷰 끄기
        SetActivePrivateCamera(false);

        selectedButtons.CharacterMoveEnd();

        IsMoving = false;
    }

    // 손 코스튬 초기화
    public void ElbowCostumeSetting()
    {
        // WeaponR_locator
        /*
         * ChracterObject -> Root_M(2) -> Spine1_M(2) -> Chest_M(0) -> Scapula_R(3) -> Shoulder_R(0) -> Elbow_R(0) -> Wrist_R(0) -> WoaponR_locator(4)
         */

        // Elbow  코스튬을 가지고 있는 부모 오브젝트 가져오기
        elbowCostume = characterChestObject.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetChild(4).gameObject;

        // 손 코스튬 모두 비활성화
        for (int i = 0; i < elbowCostume.transform.childCount; i++)
        {
            elbowCostume.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // 손 코스튬 랜덤 착용
    public void ElbowCostumeEquip()
    {
        int ran = Random.Range(0, elbowCostume.transform.childCount);

        elbowCostume.transform.GetChild(ran).gameObject.SetActive(true);

        haveCostume = true;
    }

    // 캐릭터 터치시 반응 애니매이션
    IEnumerator CharacterTouchAnimation()
    {

        yield return new WaitForSeconds(0.1f);

        int ranNum = Random.Range(2, 22);

        // 움직이는 상태가 아닐때만 실행
        if (!isMove)
        {
            ani.SetInteger("animation", ranNum);
        }

        // 현재 실행중인 animation의 클립 길이를 들고온다.
        RuntimeAnimatorController rac = ani.runtimeAnimatorController;

        float playTime = rac.animationClips[ranNum].length;// - 0.5f;

        // 들고 온 클립 시간 만큼 지연 시간 할당
        yield return new WaitForSeconds(playTime);

        ani.SetInteger("animation", 1);

    }

    // 캐릭터 터치 이벤트 - 외부 호출가능한 메소드
    public void CharacterTouch()
    {
        StartCoroutine(CharacterTouchAnimation());
    }

    // 캐릭터가 무기 없이 Monster 를 만났을때 이벤트 메소드
    IEnumerator CharacterRestart(GameObject monster)
    {
        capsuleCollider.enabled = false;

        #region 이슬기찬 변경 코드
        if (TotalParameter.Instance.persons == 1)
        {
            Coding.UIController.Instance.fail.gameObject.SetActive(true);
            Coding.UIController.Instance.uibackground[0].gameObject.SetActive(false);
            Coding.UIController.Instance.gamesceneUI.gameObject.SetActive(false);
            Coding.UIController.Instance.characterView[0].gameObject.SetActive(false);
            SetActivePrivateCamera(false);
        }
        #endregion
        yield return new WaitForSeconds(0.5f);

        // 몬스터 Attack 애니매이션
        monster.GetComponent<MonsterAction>().CharacterDie(true, transform.gameObject);
        Coding.ContentsController.Instance.SoundEffect(3);

        // Die 애니메이션(6 or 7) 실행 
        ani.SetInteger("animation", 6);

        yield return new WaitForSeconds(1.2f);

        capsuleCollider.enabled = true;

        // sound 실행
        Coding.ContentsController.Instance.SoundEffect(4);

        yield return new WaitForSeconds(1.0f);

        // 이동 끝나면 호출되는 메소드
        StartCoroutine(IMoveEnd(transform.gameObject));

        yield return new WaitForSeconds(2.0f);

        // 시작 row, column 값으로 초기화
        rowNum = startRow;
        columnNum = startColumn;


        //  2인이면 시작지점으로, 코스튬 착용 경고 or 1인이면 게임 오버
        Vector3 vec = CharacterSourceContainer.Instance.tiles[rowNum].tilePositions[columnNum];
        transform.position = new Vector3(vec.x, transform.position.y, vec.z);

        // 2인 게임 이면
        if (TotalParameter.Instance.persons == 2)
        {
            // 캐릭터 죽기, 몬스터 액션 호출
            monster.gameObject.GetComponent<MonsterAction>().CharacterDie(false, transform.gameObject);
            monster.gameObject.GetComponent<MonsterAction>().MonsterPlayAnimation();
            yield break;
        }
        // 1인 게임 이면
        else if (TotalParameter.Instance.persons == 1)
        {
            // 몬스터 액션 호출
            monster.gameObject.GetComponent<MonsterAction>().MonsterPlayAnimation();
        }

    }

    // 캐릭터가 무기를 들고 Monster 를 만났을때 이벤트 메소드
    IEnumerator CharacterAttackMonster(GameObject monster)
    {
        capsuleCollider.enabled = false;

        yield return new WaitForSeconds(1.0f);

        // Player Attack 애니매이션
        ani.SetInteger("animation", 13);
        // Attack Sound 재생
        Coding.ContentsController.Instance.SoundEffect(3);

        yield return new WaitForSeconds(0.5f);

        // 몬스터 die 애니매이션
        monster.GetComponent<MonsterAction>().MonsterDie(true, transform.gameObject);

        yield return new WaitForSeconds(1.0f);

        print("몬스터 꽥");

        // 몬스터 비활성화
        monster.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        // 캐릭터 이동 가능
        isMove = true;

        yield return new WaitForSeconds(1.0f);

        capsuleCollider.enabled = true;

        // 이동 횟수 변경
        moveCount++;
        Coding.UIController.Instance.moveCountText[playerNumber].text = moveCount.ToString();

        // 남은 Arrow가 존재 할 경우 이동
        // 이동 해야 할 arrow 가 더 있으면 다시 코루틴 호출
        if (arrowTotalCount - 1 > 0)
        {
            // 이동 해야 할 arrow 값 차감
            arrowTotalCount--;

            // arrowIndex 증가
            arrowCount++;

            StartCoroutine(IPlayerMoving(transform.gameObject));
        }
        else
        {
            //추가된 부분
            SetActivePrivateCamera(isMove);
            // 이동 끝
            StartCoroutine(IMoveEnd(transform.gameObject));
        }
    }

    // 벽 부딪혔을때 발생하는 이벤트
    IEnumerator WallEvent()
    {
        // Damage 애니메이션(5) 실행 
        ani.SetInteger("animation", 5);

        yield return new WaitForSeconds(1.0f);

        // 이동 끝나면 호출되는 메소드
        StartCoroutine(IMoveEnd(transform.gameObject));

        // Idle 애니메이션(1) 실행 
        ani.SetInteger("animation", 1);

        // 위치값이 틀어졌을 경우를 위해 캐릭터가 있어야할 position값을 재할당
        Vector3 vec = CharacterSourceContainer.Instance.tiles[rowNum].tilePositions[columnNum];
        transform.position = new Vector3(vec.x, transform.position.y, vec.z);

        Coding.ContentsController.Instance.SoundEffect(1);
    }

    // 충돌 처리
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Bumb"))
        {
            // 캐릭터 현재 row, column 값을 시작값으로 초기화
            rowNum = startRow;
            columnNum = startColumn;

            // Die 애니메이션(6 or 7) 실행 
            ani.SetInteger("animation", 6);
            capsuleCollider.enabled = false;

            StopAllCoroutines();

            // 이동 불가능
            isMove = false;

            // 2인 게임 이면
            if (TotalParameter.Instance.persons == 2)
            {
                // 이동 횟수 변경
                moveCount++;
                Coding.UIController.Instance.moveCountText[playerNumber].text = moveCount.ToString();

                // 이동 끝나면 호출되는 메소드
                StartCoroutine(IMoveEnd(transform.gameObject));

            }
            // 1인 게임 이면
            else if (TotalParameter.Instance.persons == 1)
            {
                // Fail 띄우기, 다시 시작
                //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
            print("폭탄 맞고 화면 끄기");
        }
        else if (other.CompareTag("Final"))
        {
            //if (other.gameObject.layer == layerMask)
            //{
            print("승리!!");

            rowNum = lastRow;
            columnNum = lastColumn;

            // Victory 애니메이션 실행
            ani.SetInteger("animation", 3);

            selectedButtons.CharacterMoveEnd();

            // 2명이 동시에 골인 지점에 도착하여 캐릭터 싸움 event 실행 방지를 위해 콜라이더 비활성화
            gameObject.GetComponent<CapsuleCollider>().enabled = false;

            // 이동 횟수 UI, Text 비활성화
            for (int i = 0; i < Coding.UIController.Instance.moveImg.Length; i++)
            {
                Coding.UIController.Instance.moveImg[i].SetActive(false);
                Coding.UIController.Instance.moveCountText[i].enabled = false;
            }
        }
        // 손 코스튬 착용
        else if (other.CompareTag("Costume"))
        {
            print("코스튬입자~");

            other.enabled = false;

            // ATK3 애니메이션 실행
            ani.SetInteger("animation", 13);

            // 손 코스튬 초기화 및 착용
            ElbowCostumeSetting();
            ElbowCostumeEquip();
        }
        else if (other.CompareTag("Player"))
        {
            print("플레이어 쾅");

            if (ani.GetInteger("animation") != 6 &&              // 사망 여부 확인
                !IsMoving &&                                     // 이동 중이 었는지 확인
                (rowNum != lastRow || columnNum != lastColumn))  // 정지 상태 였는지 확인
            {
                rowNum = lastRow;
                columnNum = lastColumn;

                moveCount++;
                Coding.UIController.Instance.moveCountText[playerNumber].text = moveCount.ToString();

                print("너랑 나랑 다퉈~~");
                SetActivePrivateCamera(false);

                // 다음 Arrow 멈추기
                StopAllCoroutines();

                // 충돌 player 처다보기
                transform.LookAt(other.transform);

                // ATK1 애니메이션 실행
                ani.SetInteger("animation", 23);

                // 이동 끝나면 호출되는 메소드
                selectedButtons.CharacterMoveEnd();
            }
        }
        else if (other.CompareTag("Wall"))
        {
            // 다음 Arrow 멈추기
            StopAllCoroutines();

            print("벽이지롱~");
            SetActivePrivateCamera(false);

            // 이동을 멈춤
            isMove = false;

            StartCoroutine(WallEvent());

        }
        else if (other.CompareTag("Monster"))
        {
            other.GetComponent<BoxCollider>().enabled = false;

            if (haveCostume)
            {
                // 이동 정지
                isMove = false;

                StopAllCoroutines();

                rowNum = lastRow;
                columnNum = lastColumn;

                StartCoroutine(CharacterAttackMonster(other.gameObject));
            }
            else if (!haveCostume)
            {
                moveCount++;
                Coding.UIController.Instance.moveCountText[playerNumber].text = moveCount.ToString();

                // 이동 정지
                isMove = false;

                // 캐릭터 die
                StopAllCoroutines();

                StartCoroutine(CharacterRestart(other.gameObject));

                print("도구가 없네.. 캐릭터 꽥");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어 두명이 같은칸에 있는동안
            // ATK1 애니메이션 실행
            ani.SetInteger("animation", 23);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어 끼리 충돌 후 벗어 나게 되면 애니메이션 다시 Idle 상태로 변경
        if (other.CompareTag("Player"))
        {
            print("플레이어 쾅 끝");

            print("너랑 나랑 화해~.~");

            ani.SetInteger("animation", 1);
        }
    }
}
