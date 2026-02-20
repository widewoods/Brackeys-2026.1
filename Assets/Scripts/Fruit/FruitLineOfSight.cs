using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitLineOfSight : MonoBehaviour
{
    [SerializeField] private float coneAngleDeg;
    [SerializeField] private float distance;
    [SerializeField] private int rayCount;

    public bool hasLineOfSight;

    void FixedUpdate()
    {
        CastSightCone(coneAngleDeg, distance, rayCount);
    }

    void CastSightCone(float coneAngleDeg, float distance, int rayCount)
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
                Debug.DrawLine(transform.position, hit.point, Color.green);
                hasLineOfSight = true;
            }
            else
            {
                Debug.DrawRay(transform.position, direction.normalized * distance, Color.red);
            }
        }
    }
}
