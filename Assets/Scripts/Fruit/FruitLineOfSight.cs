using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitLineOfSight : MonoBehaviour, ITargetSensor
{
    [SerializeField] private float coneAngleDeg;
    [SerializeField] private float distance;
    [SerializeField] private int rayCount;

    private float originalDistance;

    [SerializeField] private LayerMask layerMask;

    public bool hasLineOfSight;

    void OnEnable()
    {
        PlayerController.OnHidingChanged += HandleHiding;
    }

    void OnDisable()
    {
        PlayerController.OnHidingChanged -= HandleHiding;
    }

    void Start()
    {
        originalDistance = distance;
    }

    Transform CastSightCone(float coneAngleDeg, float distance, int rayCount)
    {
        float half = coneAngleDeg * 0.5f;
        hasLineOfSight = false;
        for (int i = 0; i < rayCount; i++)
        {
            float t = (rayCount == 1) ? 0.5f : (float)i / (rayCount - 1);
            float angle = Mathf.Lerp(-half, half, t);

            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance))
            {
                if (!hit.collider.gameObject.CompareTag("Player")) return null;
                Debug.DrawLine(transform.position, hit.point, Color.green);
                return hit.collider.transform;
            }
            else
            {
                Debug.DrawRay(transform.position, direction.normalized * distance, Color.red);
            }
        }
        return null;
    }

    public Transform AcquireTarget()
    {
        return CastSightCone(coneAngleDeg, distance, rayCount);
    }

    private void HandleHiding(bool isHiding)
    {
        distance = isHiding ? originalDistance * 0.7f : originalDistance;
    }
}
