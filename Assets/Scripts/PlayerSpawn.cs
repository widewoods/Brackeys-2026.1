using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public static Transform SpawnTransform;

    void Awake()
    {
        SpawnTransform = transform;
    }
}
