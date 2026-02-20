using UnityEngine;

public class FruitController : MonoBehaviour
{
    public ITargetSensor Sensor { get; private set; }
    public IMover Mover { get; private set; }
    public IAttack Attack { get; private set; }
    public FruitStats Stats;

    Transform target;

    void Awake()
    {
        Sensor = GetComponent<ITargetSensor>();
        Mover = GetComponent<IMover>();
        Attack = GetComponent<IAttack>();
    }

    void Update()
    {
        target = Sensor.AcquireTarget();

        if (target == null)
        {
            Mover.Idle();
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        if (Attack.CanAttack(target, dist))
        {
            Mover.StopForAttack();
            Attack.Attack();
        }
        else
        {
            Mover.MoveTo(target.position);
        }
    }
}