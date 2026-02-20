using UnityEngine;

public interface IMover
{
    void MoveTo(Vector3 worldPos);
    void Idle();
    void StopForAttack();
    bool IsStunned { get; }
}

public interface IAttack
{
    bool CanAttack(Transform target, float distance);
    void Attack();
}

public interface ITargetSensor
{
    Transform AcquireTarget();
}