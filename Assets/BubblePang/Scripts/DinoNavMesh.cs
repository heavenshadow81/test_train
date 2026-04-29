using UnityEngine;
using UnityEngine.AI;

public class DinoNavMesh : MonoBehaviour
{
    private const float SelfIntroduceTime = 3.0f;
    private const float WaitTime = 1.0f;

    // Object 타켓들
    [SerializeField]
    private Transform[] targets;

    // Object 현재 타켓
    [SerializeField]
    private Transform target;


    // Object 특성(지상, 해양, 공중) 구분값
    public Where where;

    NavMeshAgent agent;

    AudioSource audioSource;

    // 공룡 Index Number
    public int number;

    // 애니매이션 컨트롤러
    Animator ani;
    private State state;

    private float remainWaitingTime;
    private float remainSelfIntroducingTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        ani = GetComponent<Animator>();

        remainWaitingTime = 0.0f;
        remainSelfIntroducingTime = 0.0f;
    }

    // 타켓 지정
    public void SetTargets(Transform[] targets)
    {
        this.targets = targets;
    }

    void Update()
    {
        if (!ani.GetCurrentAnimatorStateInfo(0).IsName("walk")
            && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            ani.SetInteger("whatAction", 0);
        }

        switch (state)
        {
            case State.Waiting:
                if (remainWaitingTime > 0.0f)
                {
                    remainWaitingTime -= Time.deltaTime;
                    if (remainWaitingTime <= 0.0f)
                    {
                        remainWaitingTime = 0.0f;

                        DoRandomAction();
                    }
                }
                break;
            case State.Moving:
                break;
            case State.SelfIntroducing:
                if (remainSelfIntroducingTime > 0.0f)
                {
                    remainSelfIntroducingTime -= Time.deltaTime;
                    if (remainSelfIntroducingTime <= 0.0f)
                    {
                        remainSelfIntroducingTime = 0.0f;

                        MoveToRandomDestination();
                    }
                }
                break;
        }
    }

    public void OnTouched()
    {
        if (state == State.SelfIntroducing) { return; }

        DoSelfIntroduce();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Is collide with another dino?
        if (other.tag != "Dino") { return; }
        DoRandomAction();
    }

    private void OnTriggerStay(Collider other)
    {
        if (state == State.SelfIntroducing) { return; }
        if (!IsArrivedAtDestination(other)) { return; }

        DoRandomAction();
    }

    private void DoRandomAction()
    {
        if (Random.Range(0, 2) == 0) { Wait(); }
        else { MoveToRandomDestination(); }
    }

    private bool IsArrivedAtDestination(Collider other)
    {
        return other.tag == "TargetPoint" && target != null && other.gameObject == target.transform.gameObject;
    }

    public void MoveToRandomDestination()
    {
        SetState(State.Moving);

        var ran = Random.Range(0, targets.Length);
        target = targets[ran];
        agent.SetDestination(targets[ran].position);
        ani.SetInteger("whatAction", 0);
    }

    public void DoSelfIntroduce()
    {
        SetState(State.SelfIntroducing);

        remainSelfIntroducingTime = SelfIntroduceTime;

        agent.SetDestination(transform.position);

        int ran = Random.Range(1, 5);
        ani.SetInteger("whatAction", ran);
        gameObject.transform.LookAt(Camera.main.transform.position);
        audioSource.PlayOneShot(UIResources.Instance.currentclip[ContentsController.Instance.contentsParameter.shufflepart[number]]);
    }

    private void SetActiveNamePanel(bool value)
    {
        transform.GetChild(2).gameObject.SetActive(value);
    }

    public void Wait()
    {
        SetState(State.Waiting);

        remainWaitingTime = WaitTime;

        ani.SetInteger("whatAction", 0);
    }

    private void SetState(State state)
    {
        this.state = state;

        agent.isStopped = state != State.Moving;
        SetActiveNamePanel(state == State.SelfIntroducing);
    }

    private enum State
    {
        Moving,
        SelfIntroducing,
        Waiting
    }
}
