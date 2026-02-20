using UnityEngine;

public class FruitController : MonoBehaviour
{

    public enum State { IdleHome, Chase, ReturnHome, Grabbed }

    public ITargetSensor Sensor { get; private set; }
    public IMover Mover { get; private set; }
    public FruitStats Stats;
    public State currentState;
    private State previousState;

    Transform target;

    [Header("Aggro")]
    [SerializeField] private float deaggroRange;
    [SerializeField] private float loseSightTime;
    private float lastSeenTime;

    [Header("Home")]
    [SerializeField] private Transform home;
    [SerializeField] private float returnHomeRadius;
    [SerializeField] private float farFromHomeRadius;

    void Awake()
    {
        Sensor = GetComponent<ITargetSensor>();
        Mover = GetComponent<IMover>();
        // Attack = GetComponent<IAttack>();
    }

    void Update()
    {
        var seen = Sensor.AcquireTarget();
        if (seen != null)
        {
            target = seen;
            lastSeenTime = Time.time;
        }

        switch (currentState)
        {
            case State.IdleHome:
                Mover.Idle(); // 집에 있을때 행동

                if (target != null) currentState = State.Chase; // 만약 플레이어가 보이면 Chase 상태로 변경
                break;
            case State.Chase:
                if (target == null)
                {
                    currentState = State.ReturnHome;
                    break;
                }



                // 플레이어랑 너무 멀어지면 ReturnHome으로 변경
                float distanceFromPlayer = (transform.position - target.position).magnitude;
                // bool tooFar = distanceFromPlayer >= deaggroRange;
                bool lostTooLong = (Time.time - lastSeenTime) >= loseSightTime;
                if (lostTooLong)
                {
                    target = null;
                    currentState = State.ReturnHome;
                    break;
                }

                // 집에서 멀어지면 ReturnHome 으로 변경
                float distanceFromHome = (transform.position - home.position).magnitude;
                if (distanceFromHome >= farFromHomeRadius)
                {
                    currentState = State.ReturnHome;
                    break;
                }

                // 쫓기
                Mover.MoveTo(target.position);

                break;
            case State.ReturnHome:
                // 집 돌아가기
                float dist = (transform.position - home.position).magnitude;
                if (dist <= returnHomeRadius)
                {
                    currentState = State.IdleHome;
                    break;
                }
                Mover.MoveTo(home.position);

                // 집에 돌아가는 도중 다시 보이면 다시 Chase 로 변경
                if (seen != null) currentState = State.Chase;

                break;
            case State.Grabbed:

                // 잡히는 로직 처리
                break;
        }
    }

    public void EnterGrabbed()
    {
        previousState = currentState;
        currentState = State.Grabbed;
    }

    public void ExitGrabbed()
    {
        currentState = previousState;
    }
}