using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
//완성된 오브젝트들
public class CompletedObjects : MonoBehaviour
{
    #region 변수
    static CompletedObjects _instant;
    public static CompletedObjects Instance { get => _instant; }
    //이동할 위치들 ....
    [SerializeField]
    Transform[] way;

    AudioSource audioSource;

    #endregion

    #region 유니티 함수
    private void Awake()
    {
        if (!_instant)
        {
            _instant = this;
        }
    }
    //시작시 버튼 리스트 뒤집기...!
    private void Start()
    {
        //ContentsController.Instance.Initialize += HiddenButton;

        audioSource = GetComponent<AudioSource>();
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    #endregion
    #region 함수

    public void ShowObject(int order, Vector3 position)
    {
        StartCoroutine(IShowObject(order, position));
    }
    IEnumerator IShowObject(int order, Vector3 position)
    {
        GameObject obj;


        // 공룡 Object 생성
        if (ContentsController.Instance.contentsParameter.theme == Theme.Dino)
        {
            obj = Instantiate(UIResources.Instance.currentPrefabs[ContentsController.Instance.contentsParameter.shufflepart[order]], position, Quaternion.identity);
            obj.transform.localScale = Vector3.one * 8;
        }
        // 동물 Object 생성
        else if (ContentsController.Instance.contentsParameter.theme == Theme.Animal)
        {
            obj = Instantiate(UIResources.Instance.currentPrefabs[ContentsController.Instance.contentsParameter.shufflepart[order]]);
            obj.transform.localScale = Vector3.one * 2f;
        }
        else
        {
            obj = Instantiate(UIResources.Instance.currentPrefabs[ContentsController.Instance.contentsParameter.shufflepart[order]]);
            obj.transform.localScale = Vector3.one * 5.0f;
        }
        obj.SetActive(false);
        // 생성 위치
        Transform createPosition;

        // 타켓 경로
        Transform[] trs;

        // 플레이 인원수를 들고 온다
        int person = ContentsController.Instance.contentsParameter.person;

        print("1person ?? " + ContentsController.Instance.contentsParameter.person);
        print("2person ?? " + person);

        // 생성 object가 가지고 있는 where enum 값에 따라서 경로를 다르게 지정
        switch (obj.GetComponent<DinoNavMesh>().where)
        {
            // 지상
            case Where.Ground:
                createPosition = MonsterPath.Instance.groundCreateTr[person];
                trs = MonsterPath.Instance.groundWaypoint[0].loadTr;
                break;
            // 해양
            case Where.Sea:
                createPosition = MonsterPath.Instance.seaCreateTr[person];
                trs = MonsterPath.Instance.seaWaypoint[0].loadTr;
                break;
            // 공중
            case Where.Fly:
                createPosition = MonsterPath.Instance.flyCreateTr[person];
                trs = MonsterPath.Instance.flyWaypoint[0].loadTr;
                break;
            // 배경
            case Where.back:
                createPosition = MonsterPath.Instance.groundCreateTr[person];
                trs = MonsterPath.Instance.backWaypoint[0].loadTr;
                break;
            default:
                createPosition = MonsterPath.Instance.groundCreateTr[person];
                trs = MonsterPath.Instance.groundWaypoint[0].loadTr;
                break;
        }
        // 생성 이펙트 실행
        var a = Instantiate(UIResources.Instance.createEffect[0]);
        a.transform.position = createPosition.position;
        a.Play();

        yield return new WaitForSeconds(1.0f);

        obj.SetActive(true);

        // 오브젝트 이름 할당
        obj.name = UIResources.Instance.currentPrefabs[ContentsController.Instance.contentsParameter.shufflepart[order]].name;
        // 생성된 오브젝트 경로 값을 보내줌
        obj.GetComponent<DinoNavMesh>().SetTargets(trs);
        // 오브젝트 타켓 설정 및 움직이기
        //obj.GetComponent<DinoNavMesh>().MoveToRandomDestination();
        // 오브젝트 이름표 띄워주기
        obj.GetComponent<DinoNavMesh>().DoSelfIntroduce();
        // 생성된 오브젝트 위치를 맞춘 플레이어의 오른쪽에 위치한 오브젝트에서 생성되도록 지정
        obj.transform.position = createPosition.position;

        // 오브젝트가 카메라쪽으로 바라보도록
        obj.transform.LookAt(position);
        // 오브젝트 순서 보내줌
        obj.GetComponent<DinoNavMesh>().number = order;

        StartCoroutine(MoveInitialPos(order, obj));
    }
    //이동> 재귀함수
    IEnumerator MoveInitialPos(int order, GameObject objec)
    {
        // 오브젝트 TTS 재생
        audioSource.PlayOneShot(UIResources.Instance.currentTTSClip[ContentsController.Instance.contentsParameter.shufflepart[order]]);

        yield return new WaitForSeconds(2.0f);
    }
    #endregion
}
