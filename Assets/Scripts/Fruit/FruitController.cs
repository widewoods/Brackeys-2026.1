using UnityEngine;

public class FruitController : MonoBehaviour
{

    public enum State { IdleHome, Chase, ReturnHome, Grabbed }

    public ITargetSensor Sensor { get; private set; }
    public IMover Mover { get; private set; }
    // public IAttack Attack { get; private set; }
    public FruitStats Stats;
    public State currentState;
    private State previousState;

    Transform target;

    void Awake()
    {
        Sensor = GetComponent<ITargetSensor>();
        Mover = GetComponent<IMover>();
        // Attack = GetComponent<IAttack>();
    }

    void Update()
    {
        target = Sensor.AcquireTarget();

        if (target == null)
        {
            Mover.Idle();
            return;
        }

        switch (currentState)
        {
            case State.IdleHome:
                Mover.Idle(); // 집에 있을때 행동

                if(target != null) currentState = State.Chase; // 만약 플레이어가 보이면 Chase 상태로 변경
                break;
            case State.Chase:
                // 집에서 멀어지면 ReturnHome 으로 변경

                // 플레이어랑 너무 멀어지면 ReturnHome으로 변경
                
                // 어떤 로직 통과하면 쫓으면서 공격


                break;
            case State.ReturnHome:

                // 집에 돌아가는 도중 다시 보이면 다시 Chase 로 변경
                if(target != null) currentState = State.Chase;

                break;
            case State.Grabbed:

                // 잡는 로직 처리
                break;
        }

        Mover.MoveTo(target.position);
    }

    public void EnterGrabbed()
    {
        Debug.Log("Grabbed");
        previousState = currentState;
        currentState = State.Grabbed;
    }

    public void ExitGrabbed()
    {
        Debug.Log("Released");
        currentState = previousState;
    }
}