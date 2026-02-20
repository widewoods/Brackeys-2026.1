using UnityEngine;

public class GrabbableFruit : MonoBehaviour, IGrabbable
{
    [SerializeField] float behindDotThreshold = -0.3f;
    [SerializeField] float grabRange = 1.5f;

    FruitController brain;
    ITargetSensor sensor;
    IMover mover;

    void Awake()
    {
        brain = GetComponent<FruitController>();
        sensor = brain.Sensor;
        mover = brain.Mover;
    }

    public bool CanBeGrabbed(Transform grabber)
    {
        // 잡을 수 있는지 거리 체크
        Vector3 toGrabber = grabber.position - transform.position;
        float dist = toGrabber.magnitude;
        if (dist > grabRange) return false;

        if (brain.currentState == FruitController.State.Chase) return false;

        return true;
    }

    public void OnReleased()
    {
        brain.ExitGrabbed();
    }

    public void OnGrabbed()
    {
        brain.EnterGrabbed();
    }


}