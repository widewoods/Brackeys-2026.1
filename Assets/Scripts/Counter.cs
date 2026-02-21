using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public static Vector3 CounterPosition;

    private void Awake()
    {
        CounterPosition = transform.position;
    }
}
