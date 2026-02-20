using UnityEngine;

public interface IMover
{
    void MoveTo(Vector3 worldPos);
    void Idle();
    void StopForAttack();
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

public interface IGrabbable
{
    bool CanBeGrabbed(Transform grabber);
    void OnGrabbed();
    void OnReleased();
}