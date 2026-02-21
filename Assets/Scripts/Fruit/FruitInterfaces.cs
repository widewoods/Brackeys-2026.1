using UnityEngine;

public interface IMover
{
    void MoveTo(Vector3 worldPos);
    void Idle();
    void StopForAttack();
}

public interface ITargetSensor
{
    Transform AcquireTarget();
}

public interface IAttacker
{
    void Attack(Transform target);
    bool CanAttack();
}

public interface IGrabbable
{
    bool CanBeGrabbed(Transform grabber);
    void OnGrabbed();
    void OnReleased();
}